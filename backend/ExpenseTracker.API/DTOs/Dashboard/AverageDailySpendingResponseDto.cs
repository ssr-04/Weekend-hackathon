namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class AverageDailySpendingResponseDto
    {
        public string Period { get; set; } = string.Empty;
        public decimal AverageAmount { get; set; }
        public int TotalDays { get; set; }
    }
}