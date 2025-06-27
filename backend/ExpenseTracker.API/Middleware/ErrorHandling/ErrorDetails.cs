using System.Text.Json;

namespace ExpenseTracker.API.ErrorHandling
{
    public class ErrorDetails
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; } = string.Empty;
        public string? InnerExceptionMessage { get; set; } = string.Empty;
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}