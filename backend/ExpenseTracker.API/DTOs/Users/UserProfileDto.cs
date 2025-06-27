namespace ExpenseTracker.API.DTOs.Users
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal? MonthlyIncome { get; set; }
        public int? NumberOfDependents { get; set; }
        public string? FinancialGoals { get; set; }
    }
}