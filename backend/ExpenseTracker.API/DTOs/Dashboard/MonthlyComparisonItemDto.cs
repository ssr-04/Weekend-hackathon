namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class MonthlyComparisonItemDto
    {
        public string MonthYear { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}