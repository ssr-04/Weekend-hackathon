namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class MonthlyComparisonResponseDto
    {
        public MonthlyComparisonItemDto CurrentMonth { get; set; } = null!;
        public MonthlyComparisonItemDto PreviousMonth { get; set; } = null!;
        public decimal Difference { get; set; }
        public decimal PercentageChange { get; set; }
    }
}