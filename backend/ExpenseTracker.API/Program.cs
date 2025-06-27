using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;
using ExpenseTracker.API.Data;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;
using ExpenseTracker.API.Repositories;
using ExpenseTracker.API.Services;
using ExpenseTracker.API.ErrorHandling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.OpenApi.Models;

// Serilog is configured outside the main try-catch to log any errors during startup.
Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // A fallback logger in case configuration is not read.
                .CreateBootstrapLogger();

Log.Information("Starting up");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // KESTREL HTTPS CONFIGURATION
    builder.WebHost.ConfigureKestrel(serverOptions =>
    {
        // Listens on the port specified in launchSettings for HTTPS
        serverOptions.ListenAnyIP(7247, listenOptions =>
        {
            // Use HTTPS
            listenOptions.UseHttps(httpsOptions =>
            {
                // Enforces TLS 1.2 or 1.3
                httpsOptions.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
            });
        });

        // Also listens to the HTTP port for redirection to work
        serverOptions.ListenAnyIP(5071); 
    });

    // Serilog
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration) // Reads configuration from appsettings.json
        .ReadFrom.Services(services)                   // Allows DI services to be used in sinks/enrichers
        .Enrich.FromLogContext());                     // For contextual logging

    // Add HttpContextAccessor to services
    builder.Services.AddHttpContextAccessor(); // Acccessed in Db context

    // Adding DbContext
    builder.Services.AddDbContext<ExpenseTrackerContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    // Registering repositories
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();

    // Registering services
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IExpenseService, ExpenseService>();

    // AutoMapper config
    builder.Services.AddAutoMapper(typeof(Program).Assembly);

    // CORS
    const string corsPolicyName = "AllowFrontend";

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: corsPolicyName, policy =>
        {
            var origins = builder.Configuration["Cors:AllowedOrigins"]?
                                .Split(',', StringSplitOptions.RemoveEmptyEntries) 
                                ?? Array.Empty<string>();

            policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            
        });
    });


    //  RATE LIMITING CONFIGURATION 
    builder.Services.AddRateLimiter(options =>
    {
        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

        // The partition key will be the User ID or, as a fallback, the IP address.
        options.AddPolicy("user-policy", httpContext =>
        {
            // User ID from the claims.
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var partitionKey = !string.IsNullOrEmpty(userId) 
                ? userId 
                : httpContext.Connection.RemoteIpAddress?.ToString(); // IP fallback
            
            // Read settings from appsettings.json
            var settings = builder.Configuration.GetSection("RateLimiting");
            int permitLimit = settings.GetValue<int>("PermitLimit");
            int windowInHours = settings.GetValue<int>("WindowInHours");

            return RateLimitPartition.GetFixedWindowLimiter(partitionKey,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = permitLimit,
                    Window = TimeSpan.FromHours(windowInHours),
                    // QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    // QueueLimit = 2 -- Queing doesnt immediately rejects and sends 429 instead queues and stuck loading and not suitable for 1 hour window
                });
        });
    });

    // JWT Auth config
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Removes default 5-minute clock skew
            };
        });
        
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme.\r\n\r\nEnter your token without quotes."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id   = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });


    var app = builder.Build();

    // Should be the first middleware
    app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserId", httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Anonymous");
            diagnosticContext.Set("UserEmail", httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? string.Empty);
        };
    });

    app.UseMiddleware<ExceptionHandlingMiddleware>();


    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors("AllowFrontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseRateLimiter();


    app.MapControllers();


    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Startup failed unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}