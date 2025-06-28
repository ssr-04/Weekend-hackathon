import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Category } from '../models/category.model';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl;

  /**
   * Retrieves all categories for the authenticated user.
   */
  getMyCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(`${this.baseUrl}/Categories`);
  }

  /**
   * Creates a new custom category.
   */
  createCategory(payload: { name: string }): Observable<Category> {
    return this.http.post<Category>(`${this.baseUrl}/Categories`, payload);
  }

  /**
   * Updates the name of a custom category.
   */
  editCategory(id: string, payload: { name: string }): Observable<Category> {
    return this.http.put<Category>(`${this.baseUrl}/Categories/${id}`, payload);
  }

  /**
   * Deletes a custom category.
   */
  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/Categories/${id}`);
  }
}