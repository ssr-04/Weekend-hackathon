import { Injectable, computed, inject, signal } from '@angular/core';
import { forkJoin, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import * as Dto from '../../../app/core/models/dashboard.model';
import { DashboardService } from '../../core/services/dashboardservice';

// Define the shape of the filters our dashboard will use
export interface DashboardFilters {
  startDate?: string;
  endDate?: string;
}

@Injectable() // Provided by the DashboardPageComponent
export class DashboardDataService {
  private dashboardApiService = inject(DashboardService);

  // --- STATE SIGNALS ---
  public totalExpenses = signal<Dto.TotalExpensesMonthDto | null>(null);
  public categoryBreakdown = signal<Dto.CategoryBreakdownDto | null>(null);
  public highestExpense = signal<Dto.HighestExpenseDto | null>(null);
  public monthlyComparison = signal<Dto.MonthlyComparisonDto | null>(null);
  
  public isLoading = signal(true);

  // --- COMPUTED SIGNALS for ngx-charts ---
  public categoryChartData = computed(() => {
    const breakdown = this.categoryBreakdown();
    if (!breakdown?.breakdown || breakdown.breakdown.length === 0) return [];
    return breakdown.breakdown.map(item => ({
      name: item.categoryName,
      value: item.totalAmount
    }));
  });

  /**
   * Main method to fetch all data for the dashboard.
   */
  public fetchDashboardData(filters: DashboardFilters): void {
    this.isLoading.set(true);

    const periodFilter = { startDate: filters.startDate, endDate: filters.endDate };

    forkJoin({
      total: this.dashboardApiService.getTotalExpensesForMonth(),
      breakdown: this.dashboardApiService.getCategoryBreakdown(periodFilter),
      highest: this.dashboardApiService.getHighestExpense(periodFilter),
      comparison: this.dashboardApiService.getMonthlyComparison(),
    }).pipe(
      // The isLoading state is set to false here, regardless of success or error.
      tap(() => this.isLoading.set(false)),
      catchError(err => {
        console.error("Failed to fetch dashboard data", err);
        // On error, we can clear the data or leave it as is, but we must stop loading.
        this.isLoading.set(false);
        return of(null);
      })
    ).subscribe(response => {
      if (response) {
        this.totalExpenses.set(response.total);
        this.categoryBreakdown.set(response.breakdown);
        this.highestExpense.set(response.highest);
        this.monthlyComparison.set(response.comparison);
      }
    });
  }
}