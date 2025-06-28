import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { switchMap, map, catchError } from 'rxjs/operators';
import { UserService } from '../services/user.service';

export const onboardingGuard: CanActivateFn = () => {
  const userService = inject(UserService);
  const router = inject(Router);

  // Fetch the user's profile from the backend.
  return userService.getMyProfile().pipe(
    map(profile => {
      // The condition for onboarding is if monthlyIncome is null or not set.
      if (profile.monthlyIncome === null || profile.monthlyIncome === undefined) {
        // If income is not set, redirect to the onboarding page.
        router.navigate(['/onboarding']);
        return false; // Cancel the navigation to the intended page (e.g., /dashboard).
      }
      // If income is set, the user has been onboarded. Allow access.
      return true;
    }),
    catchError(() => {
      // If there's an error fetching the profile (e.g., token issue),
      // it's safest to redirect to login. The authGuard will likely catch this anyway.
      router.navigate(['/login']);
      return of(false);
    })
  );
};