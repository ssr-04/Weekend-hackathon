import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState, authFeatureKey } from './auth.reducer';

export const selectAuthState = createFeatureSelector<AuthState>(authFeatureKey);

export const selectAuthStatus = createSelector(
  selectAuthState,
  (state) => state.status
);

// We now derive isLoggedIn from the status
export const selectIsLoggedIn = createSelector(
  selectAuthStatus,
  (status) => status === 'success'
);

export const selectAccessToken = createSelector(
  selectAuthState,
  (state) => state.accessToken
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state) => state.error
);

export const selectUserId = createSelector(
  selectAuthState,
  (state) => {
    console.log("User Id",state.userId);
    return state.userId
  }
);
