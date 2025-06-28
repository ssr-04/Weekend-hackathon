import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { filter, take, switchMap, catchError, tap } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { AuthResponse } from '../models/auth.model';
import { Store } from '@ngrx/store';
import { AuthActions } from '../store/auth/auth.actions';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private store = inject(Store);
  private baseUrl = environment.apiBaseUrl;

  private readonly TOKEN_KEY = 'aether_access_token';
  private readonly REFRESH_TOKEN_KEY = 'aether_refresh_token';
  
  // State for handling token refresh race conditions
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  login(payload: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/Auth/login`, payload);
  }

  register(payload: any): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/Auth/register`, payload);
  }

  logout(payload: { refreshToken: string }): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/Auth/logout`, payload);
  }

  refreshToken(token: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.baseUrl}/Auth/refresh-token`, { refreshToken: token }).pipe(
      tap((authResponse) => {
        // After a successful refresh, immediately update the stored tokens
        this.setTokens(authResponse);
      })
    );
  }

  // Orchestrator for handling 401 errors by attempting a token refresh
  handle401Error(request: any, next: any): Observable<any> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const refreshToken = this.getRefreshToken();

      if (refreshToken) {
        return this.refreshToken(refreshToken).pipe(
          switchMap((authResponse: AuthResponse) => {
            this.isRefreshing = false;
            this.refreshTokenSubject.next(authResponse.accessToken);
            // Retry the original failed request with the new access token
            return next(this.addTokenHeader(request, authResponse.accessToken));
          }),
          catchError((err) => {
            this.isRefreshing = false;
            // If refresh fails, log the user out completely
            this.store.dispatch(AuthActions.logout());
            return throwError(() => err);
          })
        );
      } else {
        // No refresh token available, logout immediately
        this.store.dispatch(AuthActions.logout());
        return throwError(() => new Error("No refresh token available."));
      }
    } else {
      // If a refresh is already in progress, wait for it to complete
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(jwt => next(this.addTokenHeader(request, jwt)))
      );
    }
  }

  private addTokenHeader(request: any, token: string) {
    return request.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  // --- Token Management ---
  setTokens(authResponse: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authResponse.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, authResponse.refreshToken);
    localStorage.setItem('aether_user_id', authResponse.userId);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }
  
  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  clearTokens(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem('aether_user_id');
  }
}