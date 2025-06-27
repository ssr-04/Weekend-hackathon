namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class TotalExpensesResponseDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}