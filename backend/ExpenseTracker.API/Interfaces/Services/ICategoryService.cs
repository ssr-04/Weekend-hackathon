using ExpenseTracker.API.DTOs.Categories;
using ExpenseTracker.API.Helpers;

namespace ExpenseTracker.API.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponseDto>>> GetUserCategoriesAsync(Guid userId);
        Task<Result<CategoryResponseDto>> CreateUserCategoryAsync(Guid userId, CategoryCreateRequestDto createDto);
        Task<Result<CategoryResponseDto>> UpdateUserCategoryAsync(Guid categoryId, Guid userId, CategoryUpdateRequestDto updateDto);
        Task<Result> DeleteUserCategoryAsync(Guid categoryId, Guid userId);
    }
}