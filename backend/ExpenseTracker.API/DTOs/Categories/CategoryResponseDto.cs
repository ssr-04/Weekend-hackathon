namespace ExpenseTracker.API.DTOs.Categories
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPredefined { get; set; }
    }
}