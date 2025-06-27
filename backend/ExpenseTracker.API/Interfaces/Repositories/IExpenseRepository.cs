using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.DTOs.Common;

namespace ExpenseTracker.API.Interfaces.Repositories
{
    public interface IExpenseRepository : IGenericRepository<Expense>
    {
        Task<PagedList<Expense>> GetUserExpensesAsync(Guid userId, ExpenseFilterRequestDto filterParams);
        Task<Expense?> GetExpenseByIdForUser(Guid userId, Guid expenseId);

        // Dashboard-specific query methods
        Task<decimal> GetTotalExpensesForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<CategoryExpense>> GetExpenseBreakdownByCategoryAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<Expense?> GetHighestExpenseForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}