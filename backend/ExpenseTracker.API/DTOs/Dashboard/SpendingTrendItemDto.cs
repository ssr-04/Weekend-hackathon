namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class SpendingTrendItemDto
    {
        public string PeriodLabel { get; set; } = string.Empty; // e.g., "Jan 2025", "Q1 2025"
        public decimal TotalAmount { get; set; }
    }
}