using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;

namespace ExpenseTracker.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, ITokenService tokenService, IMapper mapper)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerDto)
        {
            var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return Result<AuthResponseDto>.Failure("An account with this email already exists.");
            }

            using var hmac = new HMACSHA512();
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email.ToLower(),
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password))
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // After registration, log the user in
            return await CreateAuthenticatedResponse(user);
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("Invalid credentials.");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Result<AuthResponseDto>.Failure("Invalid credentials.");
                }
            }

            return await CreateAuthenticatedResponse(user);
        }

        public async Task<Result<AuthResponseDto>> RefreshTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);

            if (refreshToken == null || !refreshToken.IsActive)
            {
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.");
            }

            var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
            if (user == null)
            {
                return Result<AuthResponseDto>.Failure("User not found.");
            }

            // Invalidate the used refresh token
            refreshToken.Revoked = DateTimeOffset.UtcNow;
            _refreshTokenRepository.Update(refreshToken);

            // Create and save a new refresh token
            var newRefreshToken = _tokenService.CreateRefreshToken(user.Id);
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            await _refreshTokenRepository.SaveChangesAsync();

            var accessToken = _tokenService.CreateAccessToken(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresIn = (long)(DateTime.UtcNow.AddMinutes(15) - DateTime.UtcNow).TotalSeconds,
                UserId = user.Id
            });
        }

        public async Task<Result> LogoutAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(token);
        
            if (refreshToken == null || !refreshToken.IsActive)
            {
                // Can return success even if token not found, as the goal is to be logged out.
                return Result.Success();
            }

            refreshToken.Revoked = DateTimeOffset.UtcNow;
            _refreshTokenRepository.Update(refreshToken);
            
            await _refreshTokenRepository.SaveChangesAsync();

            return Result.Success();
        }

        private async Task<Result<AuthResponseDto>> CreateAuthenticatedResponse(User user)
        {
            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken(user.Id);

            await _refreshTokenRepository.AddAsync(refreshToken);
            await _refreshTokenRepository.SaveChangesAsync();

            var authResponse = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresIn = (long)(DateTime.UtcNow.AddMinutes(15) - DateTime.UtcNow).TotalSeconds,
                UserId = user.Id
            };

            return Result<AuthResponseDto>.Success(authResponse);
        }
        
        public async Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                // This case should be rare as the user is authenticated
                return Result.Failure("User not found.");
            }

            // 1. Verify the current password
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.CurrentPassword));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Result.Failure("Incorrect current password.");
                }
            }
            
            // 2. Hash and update the new password
            using var newHmac = new HMACSHA512();
            user.PasswordSalt = newHmac.Key;
            user.PasswordHash = newHmac.ComputeHash(Encoding.UTF8.GetBytes(changePasswordDto.NewPassword));
            
            // No need to call _userRepository.UpdateAsync, EF Core tracks the changes.
            await _userRepository.SaveChangesAsync();

            return Result.Success();
        }

    }
}