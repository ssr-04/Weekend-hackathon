using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User> AddAsync(User entity);
        Task SaveChangesAsync();
        Task<User?> GetByIdAsync(Guid id);
    }
}