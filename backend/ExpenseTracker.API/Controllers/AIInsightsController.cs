using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    // [Authorize] is inherited from BaseApiController
    public class AIInsightsController : BaseApiController
    {
        private readonly IAIService _aiService;

        public AIInsightsController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyInsights()
        {
            var userId = GetCurrentUserId();
            var result = await _aiService.GetDailyInsightsAsync(userId);

            if (!result.IsSuccess)
            {
                return StatusCode(503, new { message = result.Error }); // 503 Service Unavailable is appropriate here
            }

            return Ok(result.Value);
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlyInsights()
        {
            var userId = GetCurrentUserId();
            var result = await _aiService.GetMonthlyInsightsAsync(userId);

            if (!result.IsSuccess)
            {
                return StatusCode(503, new { message = result.Error });
            }

            return Ok(result.Value);
        }
        
        [HttpGet("monthly-comparison")]
        public async Task<IActionResult> GetMonthlyComparison()
        {
            var userId = GetCurrentUserId();
            var result = await _aiService.GetMonthlyComparisonInsightsAsync(userId);
            
            if (!result.IsSuccess)
            {
                return StatusCode(503, new { message = result.Error });
            }

            return Ok(result.Value);
        }
    }
}