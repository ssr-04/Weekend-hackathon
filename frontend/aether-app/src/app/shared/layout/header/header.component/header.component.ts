import { Component, HostListener, inject, signal } from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from '../../../../core/store/auth/auth.actions';
import { selectIsLoggedIn } from '../../../../core/store/auth/auth.selectors';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, NgClass],
  templateUrl: './header.component.html',
})
export class HeaderComponent {
  private store = inject(Store);
  isLoggedIn$ = this.store.select(selectIsLoggedIn);
  
  // Use Angular Signal for reactive state
  isScrolled = signal(false);

  // Listen for the window's scroll event
  @HostListener('window:scroll', [])
  onWindowScroll() {
    // Update the signal based on scroll position
    this.isScrolled.set(window.scrollY > 10);
  }

  logout() {
    this.store.dispatch(AuthActions.logout());
  }
}