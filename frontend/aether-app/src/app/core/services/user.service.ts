import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { UserProfile } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl;

  /**
   * Corresponds to: GET /api/Users/profile
   */
  getMyProfile(): Observable<UserProfile> {
    return this.http.get<UserProfile>(`${this.baseUrl}/Users/profile`);
  }

  /**
   * Corresponds to: PUT /api/Users/profile
   */
  editMyProfile(payload: { monthlyIncome?: number, numberOfDependents?: number, financialGoals?: string }): Observable<UserProfile> {
    return this.http.put<UserProfile>(`${this.baseUrl}/Users/profile`, payload);
  }

  /**
   * Corresponds to: PUT /api/Users/change-password
   */
  changeMyPassword(payload: any): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/Users/change-password`, payload);
  }
}