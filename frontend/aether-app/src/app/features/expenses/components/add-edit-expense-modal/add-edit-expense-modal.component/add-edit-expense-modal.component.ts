import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule, formatDate } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CategorySelectInputComponent } from '../../../../../shared/components/category-select-input/category-select-input.component.ts/category-select-input.component.ts';
import { ToastrService } from 'ngx-toastr';
import { Expense } from '../../../../../core/models/expense.model.js';
import { ExpenseService } from '../../../../../core/services/expense.service.js';
import { ModalService } from '../../../../../core/services/modal.service.js';
import { ExpenseStateService } from '../../../../../core/services/expense-state.service.js';
import { finalize, map, Observable } from 'rxjs';


@Component({
  selector: 'app-add-edit-expense-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CategorySelectInputComponent],
  templateUrl: './add-edit-expense-modal.component.html',
})

export class AddEditExpenseModalComponent implements OnInit {
  
  modalService = inject(ModalService);
  private fb = inject(FormBuilder);
  private expenseService = inject(ExpenseService);
  private toastr = inject(ToastrService);
  private expenseStateService = inject(ExpenseStateService); 

  isSubmitting = signal(false);
  
  // Determine if we are editing an existing expense or adding a new one
  isEditMode = signal(false);
  expenseToEdit: Expense | null = null;

  expenseForm = this.fb.group({
    title: ['', [Validators.required, Validators.maxLength(100)]],
    amount: [null as number | null, [Validators.required, Validators.min(0.01)]],
    date: [formatDate(new Date(), 'yyyy-MM-dd', 'en-US'), [Validators.required]],
    categoryName: ['', [Validators.required]],
    description: [''],
  });

  ngOnInit(): void {
    const modalData = this.modalService.modalData$();
    if (modalData && modalData.expense) {
      this.isEditMode.set(true);
      this.expenseToEdit = modalData.expense;
      this.expenseForm.patchValue({
        ...this.expenseToEdit,
        // The date input needs a specific format
        date: formatDate(this.expenseToEdit != null ? this.expenseToEdit.date : "", 'dd-MM-yyyy', 'en-US')
      });
    }
  }

  onSubmit(): void {
    if (this.expenseForm.invalid) {
      this.toastr.error('Please fill out all required fields.');
      return;
    }
    this.isSubmitting.set(true);

    const formValue = this.expenseForm.value;
    const payload = {
      ...formValue,
      date: formatDate(formValue.date!, 'dd-MM-yyyy HH:mm', 'en-US'),
    };
    
    let request$: Observable<void>;

    if (this.isEditMode()) {
      // modifyExpense already returns Observable<void>, so no change is needed.
      request$ = this.expenseService.modifyExpense(this.expenseToEdit!.id, payload);
    } else {
      // addExpense returns Observable<Expense>. We use `map` to transform it into Observable<void>.
      request$ = this.expenseService.addExpense(payload).pipe(
        map(() => undefined) // Map the emitted Expense object to void (or undefined).
      );
    }
    
    // 2. Now, the `request$` observable is consistently of type Observable<void>,
    //    and the subscribe block is type-safe.
    request$.pipe(
      finalize(() => this.isSubmitting.set(false)) // Use finalize to always reset the submitting state
    ).subscribe({
      next: () => {
        const message = this.isEditMode() ? 'Expense updated successfully!' : 'Expense added successfully!';
        this.toastr.success(message);
        this.expenseStateService.notifyExpenseMutated();
        this.modalService.close();
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'An error occurred.');
      }
    });

  }
}