using ExpenseTracker.API.Data;
using ExpenseTracker.API.DTOs;
using ExpenseTracker.API.Models;
using ExpenseTracker.API.Helpers;
using ExpenseTracker.API.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ExpenseTracker.API.DTOs.Common;
using System.Globalization;
using ExpenseTracker.API.DTOs.Dashboard;

namespace ExpenseTracker.API.Repositories
{
    public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseTrackerContext context) : base(context)
        {
        }

        public async Task<Expense?> GetExpenseByIdForUser(Guid userId, Guid expenseId)
        {
            return await _dbSet
                .Include(e => e.Category) // Eager load the category name
                .FirstOrDefaultAsync(e => e.Id == expenseId && e.UserId == userId && !e.IsDeleted);
        }

        public async Task<PagedList<Expense>> GetUserExpensesAsync(Guid userId, ExpenseFilterRequestDto filterParams)
        {
            var query = _dbSet.Where(e => e.UserId == userId && !e.IsDeleted).Include(e => e.Category).AsQueryable();

            // --- Filtering Logic ---

            if (!string.IsNullOrWhiteSpace(filterParams.StartDate))
            {

                try
                {
                    var startDate = TimeZoneHelper.ConvertIstStringToUtc(filterParams.StartDate);
                    query = query.Where(e => e.Date >= startDate);
                }
                catch (FormatException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }

            if (!string.IsNullOrWhiteSpace(filterParams.EndDate))
            {

                try
                {
                    var endDate = TimeZoneHelper.ConvertIstStringToUtc(filterParams.EndDate);
                    query = query.Where(e => e.Date <= endDate);
                }
                catch (FormatException ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }


            if (filterParams.CategoryNames != null && filterParams.CategoryNames.Any())
            {
                // Convert the list of names to lowercase for case-insensitive comparison
                var lowerCaseCategoryNames = filterParams.CategoryNames.Select(cn => cn.ToLower()).ToList();
                query = query.Where(e => lowerCaseCategoryNames.Contains(e.Category.Name.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(filterParams.SearchTerm))
            {
                var searchTermLower = filterParams.SearchTerm.ToLower();

                query = query.Where(e =>
                    e.Title.ToLower().Contains(searchTermLower) ||
                    (e.Description != null && e.Description.ToLower().Contains(searchTermLower))
                );
            }

            if (!string.IsNullOrWhiteSpace(filterParams.PaymentMethod))
            {
                var searchTermLower = filterParams.PaymentMethod.ToLower();

                query = query.Where(e => e.PaymentMethod!.ToLower() == searchTermLower
                );
            }


            var isDescending = filterParams.SortOrder?.ToLower() == "desc";

            // Default sort order
            IOrderedQueryable<Expense> orderedQuery;

            switch (filterParams.SortBy?.ToLower())
            {
                case "date":
                    orderedQuery = isDescending
                        ? query.OrderByDescending(e => e.Date)
                        : query.OrderBy(e => e.Date);
                    break;
                case "amount":
                    orderedQuery = isDescending
                        ? query.OrderByDescending(e => e.Amount)
                        : query.OrderBy(e => e.Amount);
                    break;
                case "title":
                    orderedQuery = isDescending
                        ? query.OrderByDescending(e => e.Title)
                        : query.OrderBy(e => e.Title);
                    break;
                default:
                    orderedQuery = query.OrderByDescending(e => e.Date);
                    break;
            }


            return await PagedList<Expense>.CreateAsync(orderedQuery, filterParams.PageNumber, filterParams.PageSize);
        }

        // --- Dashboard Methods --- 

        public async Task<decimal> GetTotalExpensesForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .SumAsync(e => e.Amount);
        }

        public async Task<IEnumerable<CategoryExpense>> GetExpenseBreakdownByCategoryAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .Include(e => e.Category)
                .GroupBy(e => e.Category.Name)
                .Select(g => new CategoryExpense
                {
                    CategoryName = g.Key,
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderByDescending(r => r.TotalAmount)
                .ToListAsync();
        }

        public async Task<Expense?> GetHighestExpenseForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .OrderByDescending(e => e.Amount)
                .Include(e => e.Category) // Include category for context
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DailySpendingItemDto>> GetDailySpendingForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .GroupBy(e => e.Date.Date) // Group by the calendar date, stripping the time part
                .Select(g => new DailySpendingItemDto
                {
                    Date = new DateTimeOffset(g.Key, TimeSpan.Zero), // Ensure it's treated as a full DateTimeOffset
                    TotalAmount = g.Sum(e => e.Amount)
                })
                .OrderBy(ds => ds.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<SpendingTrendItemDto>> GetSpendingTrendAsync(
            Guid userId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            string groupBy)
        {
            var query = _dbSet.Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate);

            switch (groupBy.ToLower())
            {
                case "quarter":
                    {
                        var data = await query
                            .GroupBy(e => new { e.Date.Year, Quarter = (e.Date.Month - 1) / 3 + 1 })
                            .Select(g => new { g.Key.Year, g.Key.Quarter, TotalAmount = g.Sum(e => e.Amount) })
                            .OrderBy(r => r.Year).ThenBy(r => r.Quarter)
                            .ToListAsync();

                        return data.Select(r => new SpendingTrendItemDto
                        {
                            PeriodLabel = $"Q{r.Quarter} {r.Year}",
                            TotalAmount = r.TotalAmount
                        });
                    }

                case "month":
                default:
                    {
                        var data = await query
                            .GroupBy(e => new { e.Date.Year, e.Date.Month })
                            .Select(g => new { g.Key.Year, g.Key.Month, TotalAmount = g.Sum(e => e.Amount) })
                            .OrderBy(r => r.Year).ThenBy(r => r.Month)
                            .ToListAsync();

                        return data.Select(r => new SpendingTrendItemDto
                        {
                            PeriodLabel = new DateTime(r.Year, r.Month, 1).ToString("MMM yyyy", CultureInfo.InvariantCulture),
                            TotalAmount = r.TotalAmount
                        });
                    }
            }
        }
        
        public async Task<IEnumerable<Expense>> GetExpensesForPeriodAsync(Guid userId, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _dbSet
                .Where(e => e.UserId == userId && !e.IsDeleted && e.Date >= startDate && e.Date <= endDate)
                .ToListAsync(); // This executes the query and brings the results into memory.
        }



    }
}