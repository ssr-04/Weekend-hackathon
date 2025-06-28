import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Subject, Subscription } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { ExpenseListItemComponent } from '../../../../../shared/components/expense-list-item/expense-list-item.component/expense-list-item.component';
import { Expense } from '../../../../../core/models/expense.model';
import { ExpenseStateService } from '../../../../../core/services/expense-state.service';
import { ExpenseService } from '../../../../../core/services/expense.service';
import { ModalService } from '../../../../../core/services/modal.service';


@Component({
  selector: 'app-track-expense-page',
  standalone: true,
  imports: [CommonModule, ExpenseListItemComponent],
  templateUrl: './track-expense-page.component.html',
})
export class TrackExpensePageComponent implements OnInit, OnDestroy {
  private expenseService = inject(ExpenseService);
  private toastr = inject(ToastrService);
  private expenseStateService = inject(ExpenseStateService); // Inject the state service
  modalService = inject(ModalService);
  
  private destroy$ = new Subject<void>();
  
  todaysExpenses = signal<Expense[]>([]);
  isLoading = signal(true);
  currentDate = new Date();

  ngOnInit(): void {
    this.loadExpenses();

    // --- THE FIX ---
    // Subscribe to the mutation notifier. When it fires, reload the expenses.
    this.expenseStateService.expenseMutated$
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.toastr.info('Your expense list has been updated.', 'Live Refresh');
        this.loadExpenses();
      });
    // ---------------
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadExpenses(): void {
    this.isLoading.set(true);
    this.expenseService.getTodayExpenses().subscribe(expenses => {
      this.todaysExpenses.set(expenses.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime()));
      this.isLoading.set(false);
    });
  }

  openAddModal(): void {
    this.modalService.open({ type: 'addEditExpense' });
  }

  onEditExpense(expense: Expense): void {
    this.modalService.open({ type: 'addEditExpense', expense: expense });
  }

  onDeleteExpense(id: string): void {
    this.expenseService.deleteExpense(id).subscribe({
      next: () => {
        this.toastr.success('Expense deleted successfully.');
        this.todaysExpenses.update(expenses => expenses.filter(e => e.id !== id));
      },
      error: (err) => this.toastr.error(err.error?.message || 'Failed to delete expense.')
    });
  }
}