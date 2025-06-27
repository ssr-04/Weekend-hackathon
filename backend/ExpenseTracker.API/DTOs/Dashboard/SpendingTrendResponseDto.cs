namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class SpendingTrendResponseDto
    {
        public List<SpendingTrendItemDto> Trends { get; set; } = new();
    }
}