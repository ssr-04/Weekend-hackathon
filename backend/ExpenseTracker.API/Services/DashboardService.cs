using AutoMapper;
using ExpenseTracker.API.DTOs.Dashboard;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using ExpenseTracker.API.Interfaces.Services;
using System.Globalization;

namespace ExpenseTracker.API.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IMapper _mapper;

        public DashboardService(IExpenseRepository expenseRepository, IMapper mapper)
        {
            _expenseRepository = expenseRepository;
            _mapper = mapper;
        }

        public async Task<Result<TotalExpensesResponseDto>> GetTotalExpensesForCurrentMonthAsync(Guid userId)
        {
            var now = DateTimeOffset.UtcNow;
            var startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
            var endDate = startDate.AddMonths(1).AddTicks(-1);

            var total = await _expenseRepository.GetTotalExpensesForPeriodAsync(userId, startDate, endDate);
            
            var response = new TotalExpensesResponseDto
            {
                Period = startDate.ToString("MMMM yyyy"),
                TotalAmount = total
            };
            
            return Result<TotalExpensesResponseDto>.Success(response);
        }
        
        public async Task<Result<CategoryBreakdownResponseDto>> GetCategoryBreakdownAsync(Guid userId, string? startDateString, string? endDateString)
        {
            DateTimeOffset startDate;
            DateTimeOffset endDate;
            
            var now = DateTimeOffset.UtcNow;
            try
            {
                if (!string.IsNullOrWhiteSpace(startDateString))
                    startDate = TimeZoneHelper.ConvertIstStringToUtc(startDateString);
                else
                    startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                if (!string.IsNullOrWhiteSpace(endDateString))
                    endDate = TimeZoneHelper.ConvertIstStringToUtc(endDateString);
                else
                    endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            catch (FormatException ex)
            {
                startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                System.Console.WriteLine(ex.Message);
            }
            

            var breakdownData = await _expenseRepository.GetExpenseBreakdownByCategoryAsync(userId, startDate, endDate);
            var totalExpenses = breakdownData.Sum(b => b.TotalAmount);
            
            var breakdownItems = breakdownData.Select(b => new CategoryBreakdownItemDto
            {
                // Note: We need to get the ID from the source data for the DTO
                CategoryName = b.CategoryName,
                TotalAmount = b.TotalAmount,
                Percentage = totalExpenses > 0 ? Math.Round((double)(b.TotalAmount / totalExpenses) * 100, 2) : 0
            }).ToList();

            var response = new CategoryBreakdownResponseDto
            {
                Period = $"{startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}",
                Breakdown = breakdownItems
            };

            return Result<CategoryBreakdownResponseDto>.Success(response);
        }

        public async Task<Result<HighestExpenseResponseDto>> GetHighestExpenseAsync(Guid userId, string? startDateString, string? endDateString)
        {
            DateTimeOffset startDate;
            DateTimeOffset endDate;
            
            var now = DateTimeOffset.UtcNow;
            try
            {
                if (!string.IsNullOrWhiteSpace(startDateString))
                    startDate = TimeZoneHelper.ConvertIstStringToUtc(startDateString);
                else
                    startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                if (!string.IsNullOrWhiteSpace(endDateString))
                    endDate = TimeZoneHelper.ConvertIstStringToUtc(endDateString);
                else
                    endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            catch (FormatException ex)
            {
                startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                System.Console.WriteLine(ex.Message);
            }

            var highestExpense = await _expenseRepository.GetHighestExpenseForPeriodAsync(userId, startDate, endDate);

            if (highestExpense == null)
            {
                return Result<HighestExpenseResponseDto>.Failure("No expenses found for the specified period.");
            }
            
            var response = _mapper.Map<HighestExpenseResponseDto>(highestExpense);
            response.Period = $"{startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}";
            
            return Result<HighestExpenseResponseDto>.Success(response);
        }
        
        public async Task<Result<MonthlyComparisonResponseDto>> GetMonthlyComparisonAsync(Guid userId)
        {
            var now = DateTimeOffset.UtcNow;
            var currentMonthStart = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
            var currentMonthEnd = currentMonthStart.AddMonths(1).AddTicks(-1);

            var previousMonthStart = currentMonthStart.AddMonths(-1);
            var previousMonthEnd = currentMonthStart.AddTicks(-1);

            var currentMonthTotal = await _expenseRepository.GetTotalExpensesForPeriodAsync(userId, currentMonthStart, currentMonthEnd);
            var previousMonthTotal = await _expenseRepository.GetTotalExpensesForPeriodAsync(userId, previousMonthStart, previousMonthEnd);

            decimal difference = currentMonthTotal - previousMonthTotal;
            decimal percentageChange = previousMonthTotal > 0 ? (difference / previousMonthTotal) * 100 : (currentMonthTotal > 0 ? 100 : 0);
            
            var response = new MonthlyComparisonResponseDto
            {
                CurrentMonth = new MonthlyComparisonItemDto { MonthYear = currentMonthStart.ToString("MMMM yyyy"), TotalAmount = currentMonthTotal },
                PreviousMonth = new MonthlyComparisonItemDto { MonthYear = previousMonthStart.ToString("MMMM yyyy"), TotalAmount = previousMonthTotal },
                Difference = difference,
                PercentageChange = Math.Round(percentageChange, 2)
            };
            
            return Result<MonthlyComparisonResponseDto>.Success(response);
        }
        
        public async Task<Result<SpendingByDayOfWeekResponseDto>> GetSpendingByDayOfWeekAsync(Guid userId, string? startDateString, string? endDateString)
        {
            DateTimeOffset startDate;
            DateTimeOffset endDate;
            
            var now = DateTimeOffset.UtcNow;
            try
            {
                if (!string.IsNullOrWhiteSpace(startDateString))
                    startDate = TimeZoneHelper.ConvertIstStringToUtc(startDateString);
                else
                    startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                if (!string.IsNullOrWhiteSpace(endDateString))
                    endDate = TimeZoneHelper.ConvertIstStringToUtc(endDateString);
                else
                    endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            catch (FormatException ex)
            {
                startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                System.Console.WriteLine(ex.Message);
            }
            
            var expenses = await _expenseRepository.GetExpensesForPeriodAsync(userId, startDate, endDate);
            
            var breakdown = expenses
                .GroupBy(e => e.Date.DayOfWeek) // Group by the DayOfWeek enum
                .Select(g => new SpendingByDayOfWeekItemDto
                {
                    DayOfWeek = g.Key.ToString(), // "Monday", "Tuesday", etc.
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(item => (int)Enum.Parse<DayOfWeek>(item.DayOfWeek)) // Ensure Monday comes first
                .ToList();

            var response = new SpendingByDayOfWeekResponseDto
            {
                // You might want to format the period string here
                period = $"{startDate.ToString("dd-MMM-yyyy")} to {endDate.ToString("dd-MMM-yyyy")}",
                Breakdown = breakdown
            };

            return Result<SpendingByDayOfWeekResponseDto>.Success(response);

        }


        public async Task<Result<SpendingTrendResponseDto>> GetSpendingTrendAsync(Guid userId, int periodCount = 6, string groupBy = "Month")
        {
            var now = DateTimeOffset.UtcNow;
            // Go back N-1 months to get N total periods (e.g., for 6 months, go back 5 months to include the current one)
            var startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset).AddMonths(-(periodCount - 1));
            var endDate = now;

            var trendsData = await _expenseRepository.GetSpendingTrendAsync(userId, startDate, endDate, groupBy);

            var response = new SpendingTrendResponseDto
            {
                Trends = trendsData.Select(t => new SpendingTrendItemDto { PeriodLabel = t.PeriodLabel, TotalAmount = t.TotalAmount }).ToList()
            };

            return Result<SpendingTrendResponseDto>.Success(response);
        }

        public async Task<Result<AverageDailySpendingResponseDto>> GetAverageDailySpendingAsync(Guid userId, string? startDateString, string? endDateString)
        {
            DateTimeOffset startDate;
            DateTimeOffset endDate;
            
            var now = DateTimeOffset.UtcNow;
            try
            {
                if (!string.IsNullOrWhiteSpace(startDateString))
                    startDate = TimeZoneHelper.ConvertIstStringToUtc(startDateString);
                else
                    startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                if (!string.IsNullOrWhiteSpace(endDateString))
                    endDate = TimeZoneHelper.ConvertIstStringToUtc(endDateString);
                else
                    endDate = startDate.AddMonths(1).AddTicks(-1);
            }
            catch (FormatException ex)
            {
                startDate = new DateTimeOffset(now.Year, now.Month, 1, 0, 0, 0, now.Offset);
                endDate = startDate.AddMonths(1).AddTicks(-1);
                System.Console.WriteLine(ex.Message);
            }

            var totalAmount = await _expenseRepository.GetTotalExpensesForPeriodAsync(userId, startDate, endDate);

            var totalDays = (endDate - startDate).Days + 1;
            var averageAmount = totalDays > 0 ? totalAmount / totalDays : 0;

            var response = new AverageDailySpendingResponseDto
            {
                Period = $"{startDate:dd-MMM-yyyy} to {endDate:dd-MMM-yyyy}",
                AverageAmount = Math.Round(averageAmount, 2),
                TotalDays = totalDays
            };

            return Result<AverageDailySpendingResponseDto>.Success(response);
        }
    }
}