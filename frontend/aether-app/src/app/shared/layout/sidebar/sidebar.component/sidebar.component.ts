import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from '../../../../core/store/auth/auth.actions';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
})
export class SidebarComponent {
  private store = inject(Store);


  logout(): void {
    this.store.dispatch(AuthActions.logout());
  }


  navLinks = [
    { path: '/dashboard', label: 'Dashboard', icon: 'layout-dashboard' },
    { path: '/dashboard/track', label: 'Track Expense', icon: 'plus-circle' },
    { path: '/dashboard/expenses', label: 'View Expenses', icon: 'list' },
    { path: '/dashboard/ai-insights', label: 'AI Insights', icon: 'sparkles' },
    { path: '/dashboard/settings', label: 'Settings', icon: 'settings' },
  ];
}