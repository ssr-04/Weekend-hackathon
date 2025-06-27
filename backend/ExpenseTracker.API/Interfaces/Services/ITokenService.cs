using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface ITokenService
    {
        string CreateAccessToken(User user);
        RefreshToken CreateRefreshToken(Guid userId);
    }
}