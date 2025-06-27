using ExpenseTracker.API.DTOs.Dashboard;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<Result<TotalExpensesResponseDto>> GetTotalExpensesForCurrentMonthAsync(Guid userId);
        Task<Result<CategoryBreakdownResponseDto>> GetCategoryBreakdownAsync(Guid userId, string? startDateString, string? endDateString);
        Task<Result<HighestExpenseResponseDto>> GetHighestExpenseAsync(Guid userId, string? startDateString, string? endDateString);
        Task<Result<MonthlyComparisonResponseDto>> GetMonthlyComparisonAsync(Guid userId);
        Task<Result<SpendingTrendResponseDto>> GetSpendingTrendAsync(Guid userId, int periodCount, string groupBy);
        Task<Result<AverageDailySpendingResponseDto>> GetAverageDailySpendingAsync(Guid userId, string? startDateString, string? endDateString);
        Task<Result<SpendingByDayOfWeekResponseDto>> GetSpendingByDayOfWeekAsync(Guid userId, string? startDateString, string? endDateString);

    }
}