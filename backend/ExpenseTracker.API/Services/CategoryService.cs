using AutoMapper;
using ExpenseTracker.API.DTOs.Categories;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;

namespace ExpenseTracker.API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IExpenseRepository _expenseRepository; // Needed to check for usage before delete
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IExpenseRepository expenseRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<CategoryResponseDto>>> GetUserCategoriesAsync(Guid userId)
        {
            var categories = await _categoryRepository.GetUserCategoriesAsync(userId);
            var categoryDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
            return Result<IEnumerable<CategoryResponseDto>>.Success(categoryDtos);
        }

        public async Task<Result<CategoryResponseDto>> CreateUserCategoryAsync(Guid userId, CategoryCreateRequestDto createDto)
        {
            // Check if the user already has a custom category with the same name (case-insensitive)
            var existingCategory = await _categoryRepository.FindUserCategoryByNameAsync(userId, createDto.Name);
            if (existingCategory != null)
            {
                return Result<CategoryResponseDto>.Failure($"A category named '{createDto.Name}' already exists.");
            }

            var category = _mapper.Map<Category>(createDto);
            category.UserId = userId;
            category.IsPredefined = false; // User-created categories are never predefined

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var responseDto = _mapper.Map<CategoryResponseDto>(category);
            return Result<CategoryResponseDto>.Success(responseDto);
        }

        public async Task<Result<CategoryResponseDto>> UpdateUserCategoryAsync(Guid categoryId, Guid userId, CategoryUpdateRequestDto updateDto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null || category.UserId != userId || category.IsPredefined)
            {
                return Result<CategoryResponseDto>.Failure("Category not found or you do not have permission to update it.");
            }
            
            // Check for name collision before updating
            var existingCategory = await _categoryRepository.FindUserCategoryByNameAsync(userId, updateDto.Name);
            if (existingCategory != null && existingCategory.Id != categoryId)
            {
                return Result<CategoryResponseDto>.Failure($"Another category named '{updateDto.Name}' already exists.");
            }

            _mapper.Map(updateDto, category);
            // No need to call UpdateAsync, EF Core tracks changes
            await _categoryRepository.SaveChangesAsync();

            var responseDto = _mapper.Map<CategoryResponseDto>(category);
            return Result<CategoryResponseDto>.Success(responseDto);
        }

        public async Task<Result> DeleteUserCategoryAsync(Guid categoryId, Guid userId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);

            if (category == null || category.UserId != userId || category.IsPredefined)
            {
                return Result.Failure("Category not found or you do not have permission to delete it.");
            }

            // **Business Rule:** Check if the category is currently in use by any expense.
            var expensesWithCategory = await _expenseRepository.GetAllAsync(e => e.CategoryId == categoryId);
            if (expensesWithCategory.Any())
            {
                return Result.Failure("Cannot delete a category that is currently assigned to one or more expenses.");
            }

            var success = await _categoryRepository.DeleteAsync(categoryId); // Soft delete
            if (!success)
            {
                // This case is unlikely if the first check passes, but good for safety.
                return Result.Failure("Failed to delete the category.");
            }
            
            await _categoryRepository.SaveChangesAsync();
            return Result.Success();
        }
    }
}