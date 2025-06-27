namespace ExpenseTracker.API.DTOs.Expenses
{
    public class ExpenseResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTimeOffset Date { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}