using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    public class DashboardController : BaseApiController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("total-expenses-month")]
        public async Task<IActionResult> GetTotalExpensesForMonth()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetTotalExpensesForCurrentMonthAsync(userId);
            return Ok(result.Value);
        }

        [HttpGet("category-breakdown")]
        public async Task<IActionResult> GetCategoryBreakdown([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetCategoryBreakdownAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }

        [HttpGet("highest-expense")]
        public async Task<IActionResult> GetHighestExpense([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetHighestExpenseAsync(userId, startDate, endDate);
            if (!result.IsSuccess) return NotFound(new { message = result.Error });
            return Ok(result.Value);
        }
        
        [HttpGet("monthly-comparison")]
        public async Task<IActionResult> GetMonthlyComparison()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetMonthlyComparisonAsync(userId);
            return Ok(result.Value);
        }
        
        [HttpGet("spending-by-day-of-week")]
        public async Task<IActionResult> GetSpendingByDayOfWeek([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetSpendingByDayOfWeekAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }
        
        [HttpGet("spending-trends")]
        public async Task<IActionResult> GetSpendingTrends([FromQuery] int periodCount = 6, [FromQuery] string groupBy = "Month")
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetSpendingTrendAsync(userId, periodCount, groupBy);
            return Ok(result.Value);
        }

        [HttpGet("average-daily-spending")]
        public async Task<IActionResult> GetAverageDailySpending([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetAverageDailySpendingAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }
    }
}