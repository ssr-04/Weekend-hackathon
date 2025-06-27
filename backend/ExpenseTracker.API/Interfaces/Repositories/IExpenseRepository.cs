using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.DTOs.Common;
using ExpenseTracker.API.DTOs.Dashboard;

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
        Task<IEnumerable<SpendingTrendItemDto>> GetSpendingTrendAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate, string groupBy);
        Task<IEnumerable<DailySpendingItemDto>> GetDailySpendingForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);
        Task<IEnumerable<Expense>> GetExpensesForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate);

    }
}