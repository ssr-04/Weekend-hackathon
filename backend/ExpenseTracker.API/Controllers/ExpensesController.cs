using ExpenseTracker.API.DTOs.Common;
using ExpenseTracker.API.DTOs.Expenses;
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
        public async Task<IActionResult> GetExpenses([FromQuery] ExpenseFilterRequestDto filterParams)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.GetUserExpensesAsync(userId, filterParams);

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
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
        public async Task<IActionResult> UpdateExpense(Guid id, ExpenseUpdateRequestDto updateDto)
        {
            var userId = GetCurrentUserId();
            var result = await _expenseService.UpdateExpenseAsync(id, userId, updateDto);
            
            if (!result.IsSuccess)
            {
                // Can be 404 (not found) or 400 (bad category, bad date)
                if(result.Error!.Contains("not found"))
                    return NotFound(new { message = result.Error });

                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
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
    }
}