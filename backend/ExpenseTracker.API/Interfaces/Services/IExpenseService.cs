using ExpenseTracker.API.DTOs.Common;
using ExpenseTracker.API.DTOs.Expenses;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Models;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface IExpenseService
    {
        Task<Result<PaginatedResponseDto<ExpenseResponseDto>>> GetUserExpensesAsync(Guid userId, ExpenseFilterRequestDto filterParams);
        Task<Result<ExpenseResponseDto>> GetExpenseByIdAsync(Guid expenseId, Guid userId);
        Task<Result<ExpenseResponseDto>> CreateExpenseAsync(Guid userId, ExpenseCreateRequestDto createDto);
        Task<Result<ExpenseResponseDto>> UpdateExpenseAsync(Guid expenseId, Guid userId, ExpenseUpdateRequestDto updateDto);
        Task<Result> DeleteExpenseAsync(Guid expenseId, Guid userId);

    }
}