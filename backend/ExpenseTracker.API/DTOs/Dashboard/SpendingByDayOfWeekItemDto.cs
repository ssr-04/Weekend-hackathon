namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class SpendingByDayOfWeekItemDto
    {
        public string DayOfWeek { get; set; } = string.Empty; // e.g., "Monday"
        public decimal TotalAmount { get; set; }
    }
}