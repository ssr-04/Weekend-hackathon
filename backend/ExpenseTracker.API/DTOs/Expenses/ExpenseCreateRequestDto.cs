using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Expenses
{
    public class ExpenseCreateRequestDto
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Expense date and time is required.")]
        [RegularExpression(@"^\d{2}-\d{2}-\d{4} \d{2}:\d{2}$", ErrorMessage = "Expense date time must be in 'dd-MM-yyyy hh:mm' format.")]
        public string Date { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; } = string.Empty;


        [StringLength(50, ErrorMessage = "Payment method must be less than 50 characters.")]
        public string? PaymentMethod { get; set; }
    }
}