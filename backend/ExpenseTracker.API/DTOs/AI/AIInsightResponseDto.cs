namespace ExpenseTracker.API.DTOs.AI
{
    public class AIInsightResponseDto
    {
        public string Period { get; set; } = string.Empty;
        public string InsightText { get; set; } = string.Empty; // The AI-generated summary
    }
}