namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class DailySpendingItemDto
    {
        public DateTimeOffset Date { get; set; }

        public decimal TotalAmount { get; set; }
    }
}