using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Categories
{
    public class CategoryCreateRequestDto
    {
        [Required(ErrorMessage = "Category Name is required")]
        public string Name { get; set; } = string.Empty;
    }
}