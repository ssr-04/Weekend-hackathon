using ExpenseTracker.API.DTOs.Users;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserProfileDto>> GetUserProfileAsync(Guid userId);
        Task<Result<UserProfileDto>> UpdateUserProfileAsync(Guid userId, UpdateUserProfileRequestDto updateDto);
    }
}