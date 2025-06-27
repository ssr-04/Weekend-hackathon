using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Interfaces.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId);
        Task<Category?> FindUserCategoryByNameAsync(Guid userId, string name);
    }
}