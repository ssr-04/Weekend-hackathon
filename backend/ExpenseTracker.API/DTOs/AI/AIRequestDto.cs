namespace ExpenseTracker.API.DTOs.AI
{
    // A single expense item sent to the AI service
    public class AIExpenseItemDto
    {
        public decimal Amount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTimeOffset Date { get; set; }
    }
    
    // The main request payload sent to the Flask API
    public class AIRequestDto
    {
        public string RequestType { get; set; } = string.Empty; // e.g., "DailySummary", "MonthlyComparison"
        public List<AIExpenseItemDto> Expenses { get; set; } = new();
        
        // Optional: Sending a second set of expenses for comparison tasks
        public List<AIExpenseItemDto>? ComparisonExpenses { get; set; }
        
        // Optional: Sending user profile data for more personalized insights
        public decimal? MonthlyIncome { get; set; }
        public int? NumberOfDependents { get; set; }
    }
}