using ExpenseTracker.API.DTOs.Dashboard;
using ExpenseTracker.API.Helpers;
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
        [ProducesResponseType(typeof(Result<TotalExpensesResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTotalExpensesForMonth()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetTotalExpensesForCurrentMonthAsync(userId);
            return Ok(result.Value);
        }

        [HttpGet("category-breakdown")]
        [ProducesResponseType(typeof(Result<CategoryBreakdownResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCategoryBreakdown([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetCategoryBreakdownAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }

        [HttpGet("highest-expense")]
        [ProducesResponseType(typeof(Result<HighestExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHighestExpense([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetHighestExpenseAsync(userId, startDate, endDate);
            if (!result.IsSuccess) return NotFound(new { message = result.Error });
            return Ok(result.Value);
        }

        [HttpGet("monthly-comparison")]
        [ProducesResponseType(typeof(Result<MonthlyComparisonResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMonthlyComparison()
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetMonthlyComparisonAsync(userId);
            return Ok(result.Value);
        }

        [HttpGet("spending-by-day-of-week")]
        [ProducesResponseType(typeof(Result<SpendingByDayOfWeekResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpendingByDayOfWeek([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetSpendingByDayOfWeekAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }

        [HttpGet("spending-trends")]
        [ProducesResponseType(typeof(Result<SpendingTrendResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetSpendingTrends([FromQuery] int periodCount = 6, [FromQuery] string groupBy = "Month")
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetSpendingTrendAsync(userId, periodCount, groupBy);
            return Ok(result.Value);
        }

        [HttpGet("average-daily-spending")]
        [ProducesResponseType(typeof(Result<Result<AverageDailySpendingResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAverageDailySpending([FromQuery] string? startDate, [FromQuery] string? endDate)
        {
            var userId = GetCurrentUserId();
            var result = await _dashboardService.GetAverageDailySpendingAsync(userId, startDate, endDate);
            return Ok(result.Value);
        }
    }
}