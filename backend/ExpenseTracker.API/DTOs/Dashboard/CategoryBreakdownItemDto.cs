namespace ExpenseTracker.API.DTOs.Dashboard
{
    public class CategoryBreakdownItemDto
    {
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public double Percentage { get; set; }
    }
}
