namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class SpendingByDayOfWeekResponseDto
    {
        public string period { get; set; } = string.Empty;
        public List<SpendingByDayOfWeekItemDto> Breakdown { get; set; } = new();
    }
}