import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './shared/layout/public-layout/public-layout.component/public-layout.component';
import { LandingPageComponent } from './features/public/pages/landing-page/landing-page.component/landing-page.component';
import { AppLayoutComponent } from './shared/layout/app-layout/app-layout.component/app-layout.component';
import { authGuard } from './core/guards/auth.guard';
import { onboardingGuard } from './core/guards/onboarding.guard';

export const routes: Routes = [
  {
    path: '',
    component: PublicLayoutComponent,
    children: [
      { path: '', component: LandingPageComponent, title: 'Aether - Financial Clarity' },
      { 
        path: 'login',
        title: 'Aether - Login',
        // Load the actual component
        loadComponent: () => import('./features/auth/pages/login-page/login-page.component/login-page.component').then(m => m.LoginPageComponent) 
      },
      { 
        path: 'register',
        title: 'Aether - Register',
        // Load the actual component
        loadComponent: () => import('./features/auth/pages/register-page/register-page.component/register-page.component').then(m => m.RegisterPageComponent)
      }
    ]
  },

  {
    path: 'onboarding',
    canActivate: [authGuard], // Must be logged in to access onboarding
    loadComponent: () => import('./features/onboarding/pages/onboarding-page/onboarding-page.component/onboarding-page.component').then(m => m.OnboardingPageComponent)
  },

  // Authenticated Routes
  {
    path: 'dashboard',
    component: AppLayoutComponent,
    canActivate: [authGuard, onboardingGuard],
    children: [
      { 
        path: '', // The default route for '/dashboard'
        title: 'Aether - Dashboard',
        loadComponent: () => import('./features/dashboard/pages/dashboard-page/dashboard-page.component/dashboard-page.component').then(m => m.DashboardPageComponent) 
      },
      {
      path: 'track',
      title: 'Aether - Track Expense',
      loadComponent: () => import('./features/expenses/pages/track-expense-page/track-expense-page.component/track-expense-page.component').then(m => m.TrackExpensePageComponent)
    },
    ]
  },

  // Fallback route
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
