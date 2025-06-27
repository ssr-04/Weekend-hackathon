using ExpenseTracker.API.DTOs.Categories;
using ExpenseTracker.API.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.API.Controllers
{
    // [Authorize] is inherited from BaseApiController
    public class CategoriesController : BaseApiController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserCategories()
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.GetUserCategoriesAsync(userId);

            // This endpoint should always succeed for an authenticated user
            return Ok(result.Value);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(object), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateCategory(CategoryCreateRequestDto createDto)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.CreateUserCategoryAsync(userId, createDto);

            if (!result.IsSuccess)
            {
                // Using Conflict (409) for existing category name
                return Conflict(new { message = result.Error });
            }

            // Returning 201 Created with the location of the new resource is a REST best practice
            return CreatedAtAction(nameof(GetUserCategories), new { id = result.Value!.Id }, result.Value);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryUpdateRequestDto updateDto)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.UpdateUserCategoryAsync(id, userId, updateDto);

            if (!result.IsSuccess)
            {
                // Can be 404 (not found), 403 (no permission), or 409 (name conflict)
                // Returning 400 Bad Request is a safe general catch-all for these failures.
                return BadRequest(new { message = result.Error });
            }

            return Ok(result.Value);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userId = GetCurrentUserId();
            var result = await _categoryService.DeleteUserCategoryAsync(id, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.Error });
            }

            return NoContent();
        }
    }
}