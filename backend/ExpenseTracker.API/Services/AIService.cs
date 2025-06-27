using System.Text;
using System.Text.Json;
using ExpenseTracker.API.DTOs.AI;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.API.Services
{
    public class AIService : IAIService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AIService> _logger;

        public AIService(IHttpClientFactory httpClientFactory, IExpenseRepository expenseRepository, IUserRepository userRepository, ILogger<AIService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Result<AIInsightResponseDto>> GetDailyInsightsAsync(Guid userId)
        {
            var today = DateTimeOffset.UtcNow;
            var startOfDay = new DateTimeOffset(today.Date, TimeSpan.Zero);
            
            var expenses = await _expenseRepository.GetAllAsync(e => e.UserId == userId && e.Date >= startOfDay,  includeProperties: "Category");
            
            var requestDto = new AIRequestDto
            {
                RequestType = "DailySummary",
                Expenses = expenses.Select(e => new AIExpenseItemDto
                {
                    Amount = e.Amount,
                    CategoryName = e.Category.Name,
                    Date = e.Date
                }).ToList()
            };

            return await CallAIEndpointAsync("/insights/generate", requestDto, "Today's Spending");
        }

        public async Task<Result<AIInsightResponseDto>> GetMonthlyInsightsAsync(Guid userId)
        {
            var today = DateTimeOffset.UtcNow;
            var startOfMonth = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero);

            var expenses = await _expenseRepository.GetAllAsync(e => e.UserId == userId && e.Date >= startOfMonth,  includeProperties: "Category");
            var user = await _userRepository.GetByIdAsync(userId);

            var requestDto = new AIRequestDto
            {
                RequestType = "MonthlySummary",
                Expenses = expenses.Select(e => new AIExpenseItemDto
                {
                    Amount = e.Amount,
                    CategoryName = e.Category.Name,
                    Date = e.Date
                }).ToList(),
                MonthlyIncome = user?.MonthlyIncome,
                NumberOfDependents = user?.NumberOfDependents
            };

            return await CallAIEndpointAsync("/insights/generate", requestDto, "This Month's Spending");
        }

        public async Task<Result<AIInsightResponseDto>> GetMonthlyComparisonInsightsAsync(Guid userId)
        {
            var today = DateTimeOffset.UtcNow;
            var startOfCurrentMonth = new DateTimeOffset(today.Year, today.Month, 1, 0, 0, 0, TimeSpan.Zero);
            var startOfPreviousMonth = startOfCurrentMonth.AddMonths(-1);
            
            var currentMonthExpenses = await _expenseRepository.GetAllAsync(e => e.UserId == userId && e.Date >= startOfCurrentMonth && e.Date < today.AddDays(1),  includeProperties: "Category");
            var previousMonthExpenses = await _expenseRepository.GetAllAsync(e => e.UserId == userId && e.Date >= startOfPreviousMonth && e.Date < startOfCurrentMonth,  includeProperties: "Category");

            var requestDto = new AIRequestDto
            {
                RequestType = "MonthlyComparison",
                Expenses = currentMonthExpenses.Select(e => new AIExpenseItemDto { Amount = e.Amount, CategoryName = e.Category.Name, Date = e.Date }).ToList(),
                ComparisonExpenses = previousMonthExpenses.Select(e => new AIExpenseItemDto { Amount = e.Amount, CategoryName = e.Category.Name, Date = e.Date }).ToList()
            };
            
            return await CallAIEndpointAsync("/insights/generate", requestDto, "Current vs Previous Month");
        }

        private async Task<Result<AIInsightResponseDto>> CallAIEndpointAsync(string endpoint, AIRequestDto requestDto, string period)
        {
            var client = _httpClientFactory.CreateClient("AIServiceClient");
            var jsonPayload = JsonSerializer.Serialize(requestDto);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("AI Service returned a non-success status code {StatusCode}. Content: {ErrorContent}", response.StatusCode, errorContent);
                    return Result<AIInsightResponseDto>.Failure($"AI service failed with status: {response.StatusCode}.");
                }

                var responseStream = await response.Content.ReadAsStreamAsync();
                var aiResponse = await JsonSerializer.DeserializeAsync<AIInsightResponseDto>(responseStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (aiResponse == null)
                {
                    return Result<AIInsightResponseDto>.Failure("Failed to deserialize AI service response.");
                }
                
                // The AI service might just return the text, so we wrap it.
                aiResponse.Period = period;
                return Result<AIInsightResponseDto>.Success(aiResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while calling the AI service.");
                return Result<AIInsightResponseDto>.Failure("Could not connect to the AI service.");
            }
        }
    }
}