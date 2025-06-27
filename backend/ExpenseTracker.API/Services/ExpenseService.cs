using AutoMapper;
using ExpenseTracker.API.DTOs.Common;
using ExpenseTracker.API.DTOs.Expenses;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;

namespace ExpenseTracker.API.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository; // To validate category ownership
        private readonly IMapper _mapper;

        public ExpenseService(IExpenseRepository expenseRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        
        // This is a private helper method to avoid code duplication
        private async Task<Category> GetOrCreateCategoryByNameAsync(string categoryName, Guid userId)
        {
            // First, check for an existing user-defined category or a predefined one
            var categories = await _categoryRepository.GetUserCategoriesAsync(userId);
            var existingCategory = categories
                .FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (existingCategory != null)
            {
                return existingCategory;
            }

            // If no category exists, create a new one for the user
            var newCategory = new Category
            {
                Name = categoryName,
                UserId = userId,
                IsPredefined = false
            };

            await _categoryRepository.AddAsync(newCategory);
            // We might not save here immediately to batch it with the expense creation save.
            // For simplicity and atomicity, let's assume it's okay to save here or batch later.
            // Let's modify the service to save changes together.

            return newCategory;
        }

        public async Task<Result<ExpenseResponseDto>> CreateExpenseAsync(Guid userId, ExpenseCreateRequestDto createDto)
        {
            // *Find or create the category by name**
            var category = await GetOrCreateCategoryByNameAsync(createDto.CategoryName, userId);
            
            // This will save the new category if it was just created
            await _categoryRepository.SaveChangesAsync(); 

            var expense = _mapper.Map<Expense>(createDto);
            expense.UserId = userId;
            expense.CategoryId = category.Id; // Assign the found/created category's ID

            // Handle Date Conversion
            try
            {
                expense.Date = TimeZoneHelper.ConvertIstStringToUtc(createDto.Date);
            }
            catch(FormatException ex)
            {
                return Result<ExpenseResponseDto>.Failure(ex.Message);
            }
            
            await _expenseRepository.AddAsync(expense);
            await _expenseRepository.SaveChangesAsync();

            var createdExpense = await _expenseRepository.GetExpenseByIdForUser(userId, expense.Id);
            var responseDto = _mapper.Map<ExpenseResponseDto>(createdExpense);

            return Result<ExpenseResponseDto>.Success(responseDto);
        }


        public async Task<Result<ExpenseResponseDto>> GetExpenseByIdAsync(Guid expenseId, Guid userId)
        {
            var expense = await _expenseRepository.GetExpenseByIdForUser(userId, expenseId);
            if (expense == null)
            {
                return Result<ExpenseResponseDto>.Failure("Expense not found.");
            }

            var responseDto = _mapper.Map<ExpenseResponseDto>(expense);
            return Result<ExpenseResponseDto>.Success(responseDto);
        }

        public async Task<Result<PaginatedResponseDto<ExpenseResponseDto>>> GetUserExpensesAsync(Guid userId, ExpenseFilterRequestDto filterParams)
        {
            var pagedExpenses = await _expenseRepository.GetUserExpensesAsync(userId, filterParams);

            var expenseDtos = _mapper.Map<List<ExpenseResponseDto>>(pagedExpenses.ExpenseItems.ToList());
            
            var paginatedResponse = new PaginatedResponseDto<ExpenseResponseDto>(
                expenseDtos,
                pagedExpenses.CurrentPage,
                pagedExpenses.PageSize,
                pagedExpenses.TotalCount
            );

            
            return Result<PaginatedResponseDto<ExpenseResponseDto>>.Success(paginatedResponse);
        }

        public async Task<Result<ExpenseResponseDto>> UpdateExpenseAsync(Guid expenseId, Guid userId, ExpenseUpdateRequestDto updateDto)
        {
            var expense = await _expenseRepository.GetExpenseByIdForUser(userId, expenseId);
            if (expense == null)
            {
                return Result<ExpenseResponseDto>.Failure("Expense not found or you do not have permission to edit it.");
            }
            
            // **LOGIC CHANGE: Find or create the category if the name is being changed**
            if (!string.IsNullOrEmpty(updateDto.CategoryName) && !expense.Category.Name.Equals(updateDto.CategoryName, StringComparison.OrdinalIgnoreCase))
            {
                var newCategory = await GetOrCreateCategoryByNameAsync(updateDto.CategoryName, userId);
                expense.CategoryId = newCategory.Id;
            }

            _mapper.Map(updateDto, expense);
            
            // **Handle Date Conversion on Update**
            if (!string.IsNullOrEmpty(updateDto.Date))
            {
                try
                {
                    expense.Date = TimeZoneHelper.ConvertIstStringToUtc(updateDto.Date);
                }
                catch (FormatException ex)
                {
                    return Result<ExpenseResponseDto>.Failure(ex.Message);
                }
            }

            await _expenseRepository.SaveChangesAsync();
            
            var updatedExpense = await _expenseRepository.GetExpenseByIdForUser(userId, expense.Id);
            var responseDto = _mapper.Map<ExpenseResponseDto>(updatedExpense);

            return Result<ExpenseResponseDto>.Success(responseDto);
        }

        public async Task<Result> DeleteExpenseAsync(Guid expenseId, Guid userId)
        {
            var expense = await _expenseRepository.GetByIdAsync(expenseId);
            if (expense == null || expense.UserId != userId)
            {
                return Result.Failure("Expense not found or you do not have permission to delete it.");
            }

            await _expenseRepository.DeleteAsync(expenseId);
            await _expenseRepository.SaveChangesAsync();
            
            return Result.Success();
        }
    }
}