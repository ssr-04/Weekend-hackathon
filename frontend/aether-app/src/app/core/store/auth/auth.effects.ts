import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AuthService } from '../../services/auth.service';
import { AuthActions } from './auth.actions';
import { catchError, map, of, switchMap, take, tap } from 'rxjs';
import { Router } from '@angular/router';
import { AuthResponse } from '../../models/auth.model';
import { ToastrService } from 'ngx-toastr';

@Injectable()
export class AuthEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);
  private router = inject(Router);
  private toastr = inject(ToastrService);

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginStart),
      switchMap(action =>
        this.authService.login({ email: action.email, password: action.password }).pipe(
          map(authResponse => AuthActions.authenticationSuccess({ authResponse })),
          catchError(error =>
            of(AuthActions.authenticationFailure({ error: error.error?.message || 'Login Failed' }))
          )
        )
      )
    )
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.registerStart),
      switchMap(action =>
        this.authService.register(action.payload).pipe(
          map(authResponse => AuthActions.authenticationSuccess({ authResponse })),
          catchError(error =>
            of(AuthActions.authenticationFailure({ error: error.error?.message || 'Registration Failed' }))
          )
        )
      )
    )
  );

  autoLogin$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.autoLoginStart),
      switchMap(() => {
        const token = this.authService.getAccessToken();
        const refreshToken = this.authService.getRefreshToken();
        const userId = localStorage.getItem('aether_user_id');

        if (!token || !refreshToken || !userId) {
          return of(AuthActions.autoLoginFailure());
        }

        const authResponse: AuthResponse = {
          accessToken: token,
          refreshToken,
          userId,
          expiresIn: undefined
        };
        return of(AuthActions.authenticationSuccess({ authResponse }));
      })
    )
  );

  // Save tokens on success
  saveAuthData$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.authenticationSuccess),
      tap(({ authResponse }) => {
        this.authService.setTokens(authResponse);
      })
    ),
    { dispatch: false }
  );

  // Navigate to dashboard after login or register
  navigateToDashboard$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginStart, AuthActions.registerStart),
      switchMap(() =>
        this.actions$.pipe(
          ofType(AuthActions.authenticationSuccess),
          take(1),
          tap(() => {
            this.router.navigate(['/dashboard']);
          })
        )
      )
    ),
    { dispatch: false }
  );

  
  successToast$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.authenticationSuccess),
      tap(() => {
        this.toastr.success('Welcome back!', 'Login Successful');
      })
    ),
    { dispatch: false }
  );

  
  failureToast$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.authenticationFailure),
      tap(({ error }) => {
        this.toastr.error(error, 'Authentication Failed');
      })
    ),
    { dispatch: false }
  );

  
  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      tap(() => {
        this.authService.clearTokens();
        this.router.navigate(['/']);
        this.toastr.info('You have been logged out.', 'Session Ended');
      })
    ),
    { dispatch: false }
  );
}
