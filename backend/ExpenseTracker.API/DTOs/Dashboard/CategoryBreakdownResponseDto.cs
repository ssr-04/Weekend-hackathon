namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class CategoryBreakdownResponseDto
    {
        public string Period { get; set; } = string.Empty;
        public List<CategoryBreakdownItemDto> Breakdown { get; set; } = new();
    }
}
