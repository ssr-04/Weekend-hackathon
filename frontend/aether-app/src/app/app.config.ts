import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideState, provideStore } from '@ngrx/store';
import { provideToastr } from 'ngx-toastr';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { provideEffects } from '@ngrx/effects';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { AuthEffects } from './core/store/auth/auth.effects';
import { authFeatureKey, authReducer } from './core/store/auth/auth.reducer';
import { DashboardDataService } from './features/dashboard/dashboard-data.service';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor, errorInterceptor])),
    provideAnimations(),
    provideToastr({
      positionClass: 'toast-top-right',
      toastClass: 'nexus-toast ngx-toastr',
      preventDuplicates: true,
      closeButton: true,
      progressBar: true,
      timeOut: 5000,
    }),
    provideStore(),
    provideState(authFeatureKey, authReducer),
    provideEffects([AuthEffects]),
    DashboardDataService,

  ]
};
