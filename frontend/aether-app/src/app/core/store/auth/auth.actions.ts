import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { AuthResponse } from '../../models/auth.model';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    // New action dispatched at app startup
    'Auto Login Start': emptyProps(),
    // Action dispatched when user initiates login
    'Login Start': props<{ email: string; password: string }>(),
    'Register Start': props<{ payload: any }>(),
    // Action dispatched by effects on successful login
    'Authentication Success': props<{ authResponse: AuthResponse }>(),
    // Action dispatched by effects on failed login/auth
    'Authentication Failure': props<{ error: any }>(),
    // Action dispatched to log out
    'Logout': emptyProps(),
    // New action for when auto-login finds no token
    'Auto Login Failure': emptyProps(),
  },
});