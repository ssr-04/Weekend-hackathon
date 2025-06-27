using ExpenseTracker.API.Data;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ExpenseTrackerContext context) : base(context)
        {
        }

        public async Task<Category?> FindUserCategoryByNameAsync(Guid userId, string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => !c.IsDeleted && c.UserId == userId && c.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(Guid userId)
        {
            // Returns predefined categories (where UserId is null) OR categories created by the specific user.
            return await _dbSet
                .Where(c => !c.IsDeleted && (c.IsPredefined || c.UserId == userId))
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}