import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import * as Dto from '../models/dashboard.model'; // Using namespace import for clarity

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl;

  /**
   * Corresponds to: GET /api/Dashboard/total-expenses-month
   * Calculates and returns the sum of all expenses for the current calendar month.
   */
  getTotalExpensesForMonth(): Observable<Dto.TotalExpensesMonthDto> {
    return this.http.get<Dto.TotalExpensesMonthDto>(`${this.baseUrl}/Dashboard/total-expenses-month`);
  }
  
  /**
   * Corresponds to: GET /api/Dashboard/category-breakdown
   * Provides a breakdown of expenses by category for a given period.
   * Dates should be in a format the backend can parse, e.g., 'dd-MM-yyyy hh:mm'.
   */
  getCategoryBreakdown(filters: { startDate?: string, endDate?: string }): Observable<Dto.CategoryBreakdownDto> {
    const params = new HttpParams({ fromObject: filters as any });
    return this.http.get<Dto.CategoryBreakdownDto>(`${this.baseUrl}/Dashboard/category-breakdown`, { params });
  }

  /**
   * Corresponds to: GET /api/Dashboard/highest-expense
   * Finds and returns the single largest expense record within a given time period.
   */
  getHighestExpense(filters: { startDate?: string, endDate?: string }): Observable<Dto.HighestExpenseDto> {
    const params = new HttpParams({ fromObject: filters as any });
    return this.http.get<Dto.HighestExpenseDto>(`${this.baseUrl}/Dashboard/highest-expense`, { params });
  }

  /**
   * Corresponds to: GET /api/Dashboard/monthly-comparison
   * Returns a comparison of total spending between the current and previous calendar months.
   */
  getMonthlyComparison(): Observable<Dto.MonthlyComparisonDto> {
    return this.http.get<Dto.MonthlyComparisonDto>(`${this.baseUrl}/Dashboard/monthly-comparison`);
  }

  /**
   * Corresponds to: GET /api/Dashboard/spending-by-day-of-week
   * Aggregates total spending for each day of the week over a specified period.
   */
  getSpendingByDayOfWeek(filters: { startDate?: string, endDate?: string }): Observable<Dto.SpendingByDayOfWeekDto> {
    const params = new HttpParams({ fromObject: filters as any });
    return this.http.get<Dto.SpendingByDayOfWeekDto>(`${this.baseUrl}/Dashboard/spending-by-day-of-week`, { params });
  }

  /**
   * Corresponds to: GET /api/Dashboard/spending-trends
   * Gets aggregated spending data over a specified number of periods.
   */
  getSpendingTrends(periodCount: number = 6, groupBy: 'monthly' | 'weekly' | 'daily' = 'monthly'): Observable<Dto.SpendingTrendsDto> {
    const params = new HttpParams()
        .set('periodCount', periodCount.toString())
        .set('groupBy', groupBy);
    return this.http.get<Dto.SpendingTrendsDto>(`${this.baseUrl}/Dashboard/spending-trends`, { params });
  }

  /**
   * Corresponds to: GET /api/Dashboard/average-daily-spending
   * Calculates the average amount spent per day over a given period.
   */
  getAverageDailySpending(filters: { startDate?: string, endDate?: string }): Observable<Dto.AverageDailySpendingDto> {
    const params = new HttpParams({ fromObject: filters as any });
    return this.http.get<Dto.AverageDailySpendingDto>(`${this.baseUrl}/Dashboard/average-daily-spending`, { params });
  }
}