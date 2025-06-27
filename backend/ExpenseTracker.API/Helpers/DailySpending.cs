namespace ExpenseTracker.API.Helpers
{
    public class DailySpending
    {
        public DateTimeOffset Date { get; set; }
        public decimal TotalAmount { get; set; }
    }
}