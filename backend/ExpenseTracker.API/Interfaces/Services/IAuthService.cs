using ExpenseTracker.API.DTOs.Auth;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto registerDto);
        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto loginDto);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(string refreshToken);
        Task<Result> LogoutAsync(string refreshToken);
        Task<Result> ChangePasswordAsync(Guid userId, ChangePasswordRequestDto changePasswordDto);
    }
}