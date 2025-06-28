import { Component, effect, inject, OnDestroy, signal } from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { CustomValidators } from '../../../../../core/validators/custom-validator';
import { Subscription } from 'rxjs';
import { selectAuthError, selectAuthStatus } from '../../../../../core/store/auth/auth.selectors';
import { AuthActions } from '../../../../../core/store/auth/auth.actions';

@Component({
  selector: 'app-register-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgClass],
  templateUrl: './register-page.component.html',
})

export class RegisterPageComponent {
  private fb = inject(FormBuilder);
  private store = inject(Store);


  authError$ = this.store.select(selectAuthError);
  isSubmitting = signal(false);

  registerForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(2)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  }, { validators: CustomValidators.passwordMatching() });

  //  Password Strength Calculation ---
  passwordStrength = {
    score: 0,
    text: 'Weak',
    color: 'bg-danger'
  };

  constructor() {
    this.password?.valueChanges.subscribe(value => {
      this.updatePasswordStrength(value || '');
    });

    effect(() => {
      const status = this.store.selectSignal(selectAuthStatus)();
      if (status === 'error' || status === 'success') {
        this.isSubmitting.set(false);
      }
    });

  }

  updatePasswordStrength(password: string): void {
    let score = 0;
    if (password.length >= 8) score++;
    if (/[A-Z]/.test(password)) score++; // Uppercase
    if (/[0-9]/.test(password)) score++; // Numbers
    if (/[^A-Za-z0-9]/.test(password)) score++; // Special characters

    this.passwordStrength.score = score;

    switch (score) {
      case 1:
        this.passwordStrength.text = 'Weak';
        this.passwordStrength.color = 'bg-danger';
        break;
      case 2:
        this.passwordStrength.text = 'Fair';
        this.passwordStrength.color = 'bg-accent-gold';
        break;
      case 3:
        this.passwordStrength.text = 'Good';
        this.passwordStrength.color = 'bg-aether-blue';
        break;
      case 4:
        this.passwordStrength.text = 'Strong';
        this.passwordStrength.color = 'bg-success';
        break;
      default:
        this.passwordStrength.text = 'Weak';
        this.passwordStrength.color = 'bg-danger';
    }
  }

  onSubmit(): void {
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      return;
    }
    this.isSubmitting.set(true);
    const { ...payload } = this.registerForm.value;
    this.store.dispatch(AuthActions.registerStart({ payload }));
  }

  get name() { return this.registerForm.get('name'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }
}