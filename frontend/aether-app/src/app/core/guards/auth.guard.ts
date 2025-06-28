import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { filter, map, switchMap, take } from 'rxjs/operators';
import { selectAuthStatus, selectIsLoggedIn } from '../store/auth/auth.selectors';

export const authGuard: CanActivateFn = () => {
  const store = inject(Store);
  const router = inject(Router);

  return store.select(selectAuthStatus).pipe(
    // 1. Wait until the status is no longer 'pending' or 'loading'
    filter(status => status !== 'pending' && status !== 'loading'),
    take(1), // 2. Take the first resolved status
    // 3. Now, use a final selector to check the actual isLoggedIn state
    switchMap(() => store.select(selectIsLoggedIn)),
    map(isLoggedIn => {
      if (!isLoggedIn) {
        router.navigate(['/login']);
        return false;
      }
      return true;
    })
  );
};