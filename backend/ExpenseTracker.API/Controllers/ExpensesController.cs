using ExpenseTracker.API.DTOs.Common;
using ExpenseTracker.API.DTOs.Expenses;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    // [Authorize] is inherited from BaseApiController
    public class ExpensesController : BaseApiController
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Result<ExpenseResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateExpense(ExpenseCreateRequestDto createDto)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.CreateExpenseAsync(userId, createDto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return CreatedAtAction(nameof(GetExpenseById), new { id = result.Value!.Id }, result.Value);
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResponseDto<Result<ExpenseResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetExpenses([FromQuery] ExpenseFilterRequestDto filterParams)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.GetUserExpensesAsync(userId, filterParams);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result<ExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetExpenseById(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.GetExpenseByIdAsync(id, userId);

            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Result<ExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateExpense(Guid id, ExpenseUpdateRequestDto updateDto)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.UpdateExpenseAsync(id, userId, updateDto);

            if (!result.IsSuccess)
            {
                // Can be 404 (not found) or 400 (bad category, bad date)
                if (result.Error!.Contains("not found"))
                    return NotFound(new { message = result.Error });

                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteExpense(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.DeleteExpenseAsync(id, userId);

            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.Error });
            }

            return NoContent();
        }

        [HttpGet("today")]
        [ProducesResponseType(typeof(IEnumerable<ExpenseResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTodaysExpenses()
        {
            var userId = GetCurrentUserId();
            var now = DateTimeOffset.UtcNow;
            var startDate = new DateTimeOffset(now.Year, now.Month, now.Day, 0, 0, 0, now.Offset).ToLocalTime();
            var endDate = startDate.AddDays(1).AddTicks(-1);

            var filterParams = new DTOs.Common.ExpenseFilterRequestDto
            {
                StartDate = startDate.ToString(),
                EndDate = endDate.ToString(),
                PageNumber = 1,
                PageSize = 100 // A reasonable max for a single day's expenses
            };

            var result = await _expenseService.GetUserExpensesAsync(userId, filterParams);

            // We only want the list of items, not the pagination data for this specific endpoint
            return Ok(result.Value?.Items);
        }
    }
}