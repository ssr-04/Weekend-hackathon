using ExpenseTracker.API.DTOs.AI;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface IAIService
    {
        Task<Result<AIInsightResponseDto>> GetDailyInsightsAsync(Guid userId);
        Task<Result<AIInsightResponseDto>> GetMonthlyInsightsAsync(Guid userId);
        Task<Result<AIInsightResponseDto>> GetMonthlyComparisonInsightsAsync(Guid userId);
    }
}