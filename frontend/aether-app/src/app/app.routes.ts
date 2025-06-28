import { Routes } from '@angular/router';
import { PublicLayoutComponent } from './shared/layout/public-layout/public-layout.component/public-layout.component';
import { LandingPageComponent } from './features/public/pages/landing-page/landing-page.component/landing-page.component';
import { AppLayoutComponent } from './shared/layout/app-layout/app-layout.component/app-layout.component';
import { authGuard } from './core/guards/auth.guard';

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

  // Authenticated Routes
  {
    path: 'dashboard', // This is a simple route for now, will be expanded
    component: AppLayoutComponent,
    canActivate: [authGuard], // This will protect the route
    children: [
      // // For now, we can just load a placeholder dashboard component
      // { path: '', loadComponent: () => import('./features/dashboard/pages/dashboard-page/dashboard-page').then(m => m.DashboardPageComponent) }
    ]
  },

  // Fallback route
  { path: '**', redirectTo: '', pathMatch: 'full' }
];
