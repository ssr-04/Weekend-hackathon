import { createReducer, on } from '@ngrx/store';
import { AuthActions } from './auth.actions';

export const authFeatureKey = 'auth';

export type AuthStatus = 'pending' | 'loading' | 'success' | 'error';

export interface AuthState {
  accessToken: string | null;
  refreshToken: string | null;
  userId: string | null;
  status: AuthStatus; // Replaces isLoggedIn
  error: any | null;
}

export const initialAuthState: AuthState = {
  accessToken: null,
  refreshToken: null,
  userId: null,
  status: 'pending', // Start in 'pending' state
  error: null,
};

export const authReducer = createReducer(
  initialAuthState,
  on(AuthActions.autoLoginStart, AuthActions.loginStart, AuthActions.registerStart, (state): AuthState => ({
    ...state,
    status: 'loading',
    error: null,
  })),
  on(AuthActions.authenticationSuccess, (state, { authResponse }): AuthState => ({
    ...state,
    accessToken: authResponse.accessToken,
    refreshToken: authResponse.refreshToken,
    userId: authResponse.userId,
    status: 'success',
    error: null,
  })),
  on(AuthActions.authenticationFailure, (state, { error }): AuthState => ({
    ...state,
    ...initialAuthState, // Reset all auth state on failure
    status: 'error',
    error,
  })),
  // Auto-login failure is not a "hard" error, it just means the user isn't logged in.
  on(AuthActions.autoLoginFailure, (state): AuthState => ({
    ...state,
    ...initialAuthState,
    status: 'error', // Use 'error' status to unblock the guard
  })),
  on(AuthActions.logout, (): AuthState => ({
    ...initialAuthState,
    status: 'error', // Use 'error' to signify a non-logged-in but resolved state
  }))
);