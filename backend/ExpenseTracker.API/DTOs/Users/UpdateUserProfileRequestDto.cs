namespace ExpenseTracker.API.DTOs.Users
{
    public class UpdateUserProfileRequestDto
    {
        public decimal? MonthlyIncome { get; set; }
        public int? NumberOfDependents { get; set; }
        public string? FinancialGoals { get; set; }
    }
}