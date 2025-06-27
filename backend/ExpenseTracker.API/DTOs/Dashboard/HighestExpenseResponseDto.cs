namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class HighestExpenseResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTimeOffset Date { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Period { get; set; } = string.Empty;
    }
}
