import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AuthActions } from '../../../../../core/store/auth/auth.actions';
import { selectAuthError, selectAuthStatus } from '../../../../../core/store/auth/auth.selectors';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent {
  private fb = inject(FormBuilder);
  private store = inject(Store);

  authError$ = this.store.select(selectAuthError);
  
  // Use a signal to manage the submitting state
  isSubmitting = signal(false);

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
  });

  constructor() {
    
    effect(() => {
      const status = this.store.selectSignal(selectAuthStatus)();
      if (status === 'error' || status === 'success') {
        this.isSubmitting.set(false);
      }
    });
  }

  onSubmit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }
    this.isSubmitting.set(true); // Set state to true when submission starts
    const { email, password } = this.loginForm.value;
    if (email && password) {
      this.store.dispatch(AuthActions.loginStart({ email, password }));
    }
  }

  get email() { return this.loginForm.get('email'); }
  get password() { return this.loginForm.get('password'); }
}