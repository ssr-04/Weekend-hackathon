import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastr = inject(ToastrService);
  const authService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      // Let auth effects handle explicit login/register failures for inline messages
      if (req.url.includes('/Auth/login') || req.url.includes('/Auth/register')) {
        return throwError(() => error);
      }
      
      // If any OTHER API call returns a 401, attempt to refresh the token
      if (error.status === 401) {
        return authService.handle401Error(req, next);
      }
      
      // Handle all other non-401 errors globally
      const errorMessage = error.error?.message || error.message || 'An unknown server error occurred.';
      toastr.error(errorMessage, `Error ${error.status}`);

      return throwError(() => error);
    })
  );
};