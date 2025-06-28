/**
 * Response for GET /Dashboard/total-expenses-month
 */
export interface TotalExpensesMonthDto {
  period: string;
  totalAmount: number;
}

/**
 * A single item in the category breakdown.
 */
export interface CategoryBreakdownItem {
  categoryId: string;
  categoryName: string;
  totalAmount: number;
  percentage: number;
}

/**
 * Response for GET /Dashboard/category-breakdown
 */
export interface CategoryBreakdownDto {
  period: string;
  breakdown: CategoryBreakdownItem[];
}

/**
 * Response for GET /Dashboard/highest-expense
 */
export interface HighestExpenseDto {
  id: string;
  title: string;
  amount: number;
  date: string;
  categoryName: string;
  period: string;
}

/**
 * A single month's data for comparison.
 */
export interface MonthlyTotal {
  monthYear: string;
  totalAmount: number;
}

/**
 * Response for GET /Dashboard/monthly-comparison
 */
export interface MonthlyComparisonDto {
  currentMonth: MonthlyTotal;
  previousMonth: MonthlyTotal;
  difference: number;
  percentageChange: number;
}

/**
 * A single item for the day-of-week breakdown.
 */
export interface DayOfWeekSpending {
  dayOfWeek: string;
  totalAmount: number;
}

/**
 * Response for GET /Dashboard/spending-by-day-of-week
 */
export interface SpendingByDayOfWeekDto {
  period: string;
  breakdown: DayOfWeekSpending[];
}

/**
* A single data point for a trend chart.
*/
export interface TrendPoint {
  periodLabel: string;
  totalAmount: number;
}

/**
 * Response for GET /Dashboard/spending-trends
 */
export interface SpendingTrendsDto {
  trends: TrendPoint[];
}

/**
 * Response for GET /Dashboard/average-daily-spending
 */
export interface AverageDailySpendingDto {
  period: string;
  averageAmount: number;
  totalDays: number;
}