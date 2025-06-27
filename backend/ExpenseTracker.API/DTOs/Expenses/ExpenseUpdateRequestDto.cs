using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Expenses
{
    public class ExpenseUpdateRequestDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal? Amount { get; set; }

        [RegularExpression(@"^\d{2}-\d{2}-\d{4} \d{2}:\d{2}$", ErrorMessage = "Expense date time must be in 'dd-MM-yyyy hh:mm' format.")]
        public string? Date { get; set; } 
        public string? CategoryName { get; set; }
        public string? PaymentMethod { get; set; }
    }
}