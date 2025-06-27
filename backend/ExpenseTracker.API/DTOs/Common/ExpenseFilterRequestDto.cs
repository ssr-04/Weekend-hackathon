using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.API.DTOs.Common
{
    public class ExpenseFilterRequestDto : PaginationRequestDto
    {
        [RegularExpression(@"^\d{2}-\d{2}-\d{4} \d{2}:\d{2}$", ErrorMessage = "Expense date time must be in 'dd-MM-yyyy hh:mm' format.")]
        public string? StartDate { get; set; }

        [RegularExpression(@"^\d{2}-\d{2}-\d{4} \d{2}:\d{2}$", ErrorMessage = "Expense date time must be in 'dd-MM-yyyy hh:mm' format.")]
        public string? EndDate { get; set; }
        public List<string>? CategoryNames { get; set; }
        public string? SearchTerm { get; set; }
        public string? PaymentMethod { get; set; }
    }
}