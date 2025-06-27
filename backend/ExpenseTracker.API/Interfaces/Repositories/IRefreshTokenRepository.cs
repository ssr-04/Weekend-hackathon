using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<RefreshToken> AddAsync(RefreshToken entity);
        RefreshToken Update(RefreshToken entity);
        Task SaveChangesAsync();
    }
}