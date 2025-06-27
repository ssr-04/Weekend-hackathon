using ExpenseTracker.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Security.Claims;

namespace ExpenseTracker.API.Data
{
    public class ExpenseTrackerContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ExpenseTrackerContext(DbContextOptions<ExpenseTrackerContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        private void ApplyAuditInformation()
        {
            // Get the current user's ID from the HttpContext.
            // It might be null if the operation is performed by an unauthenticated process.
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Guid? currentUserId = !string.IsNullOrEmpty(userId) ? Guid.Parse(userId) : null;

            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added ||
                        e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                var baseEntity = (BaseEntity)entityEntry.Entity;
                baseEntity.UpdatedAt = DateTimeOffset.UtcNow;
                baseEntity.UpdatedBy = currentUserId;

                if (entityEntry.State == EntityState.Added)
                {
                    baseEntity.CreatedAt = DateTimeOffset.UtcNow;
                    baseEntity.CreatedBy = currentUserId;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // User Entity Configuration
            modelBuilder.Entity<User>(entity =>
            {
                // Ensures Email is unique across all users
                entity.HasIndex(u => u.Email).IsUnique();
            });

            // Category Entity Configuration
            modelBuilder.Entity<Category>(entity =>
            {
                // A user can have many categories with the same name, but a user's custom
                // categories should be unique by name. Predefined ones are shared.
                entity.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            });

            // Expense Entity Configuration
            modelBuilder.Entity<Expense>(entity =>
            {
                // Defining the relationship to User
                entity.HasOne(e => e.User)
                      .WithMany(u => u.Expenses)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their expenses are too.

                // Defining the relationship to Category
                entity.HasOne(e => e.Category)
                      .WithMany(c => c.Expenses)
                      .HasForeignKey(e => e.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Don't allows deleting a category if it has expenses.
            });

            // RefreshToken Entity Configuration
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasOne(rt => rt.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(rt => rt.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}