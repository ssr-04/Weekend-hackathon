import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs/operators';
import { UserService } from '../../../../../core/services/user.service';


@Component({
  selector: 'app-onboarding-page.component',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './onboarding-page.component.html',
  styleUrl: './onboarding-page.component.css'
})
export class OnboardingPageComponent {

  private fb = inject(FormBuilder);
  private userService = inject(UserService);
  private router = inject(Router);
  private toastr = inject(ToastrService);
  
  currentStep = signal(1);
  isSubmitting = signal(false);

  onboardingForm = this.fb.group({
    // Step 1
    monthlyIncome: [null as number | null, [Validators.required, Validators.min(1)]],
    // Step 2
    numberOfDependents: [null as number | null, [Validators.min(0)]],
    financialGoals: [''],
  });

  nextStep(): void {
    if (this.currentStep() === 1 && this.onboardingForm.get('monthlyIncome')?.invalid) {
      this.toastr.error('Please enter a valid monthly income to proceed.');
      return;
    }
    this.currentStep.update(step => step + 1);
  }

  prevStep(): void {
    this.currentStep.update(step => step - 1);
  }
  
  // skip(): void {
  //   // Navigate directly to the dashboard. The guard will run again,
  //   // but since they are being sent here, it will just loop.
  //   // A better UX is to let them go to the dashboard but show a persistent banner.
  //   // For now, we'll just navigate.
  //   this.toastr.info('You can complete your profile later from the Settings page.');
  //   this.router.navigate(['/dashboard']);
  // }

  onSubmit(): void {
    // Final check, although the button should be disabled.
    if (this.onboardingForm.get('monthlyIncome')?.invalid) {
      this.toastr.error('Monthly income is a required field.');
      return;
    }

    this.isSubmitting.set(true);
     // Convert nulls to undefined for API compatibility
    const raw = this.onboardingForm.value;
    const payload = {
      monthlyIncome: raw.monthlyIncome ?? undefined,
      numberOfDependents: raw.numberOfDependents ?? undefined,
      financialGoals: raw.financialGoals ?? undefined,
    };

    this.userService.editMyProfile(payload).pipe(
      finalize(() => this.isSubmitting.set(false))
    ).subscribe({
      next: () => {
        this.toastr.success('Your profile is set up. Welcome to Aether!');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Failed to save your profile.');
      }
    });
  }
}
