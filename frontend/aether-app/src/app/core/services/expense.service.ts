import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Expense, PaginatedExpenses } from '../models/expense.model';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl;

  addExpense(payload: any): Observable<Expense> {
    return this.http.post<Expense>(`${this.baseUrl}/Expenses`, payload);
  }

  getMyExpenses(filters: any): Observable<PaginatedExpenses> {
    let params = new HttpParams();
    Object.keys(filters).forEach(key => {
        if (filters[key]) {
            params = params.append(key, filters[key]);
        }
    });
    return this.http.get<PaginatedExpenses>(`${this.baseUrl}/Expenses`, { params });
  }

  getExpenseById(id: string): Observable<Expense> {
    return this.http.get<Expense>(`${this.baseUrl}/Expenses/${id}`);
  }

  modifyExpense(id: string, payload: any): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/Expenses/${id}`, payload);
  }

  deleteExpense(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Expenses/${id}`);
  }

  getTodayExpenses(): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.baseUrl}/Expenses/today`);
  }
}