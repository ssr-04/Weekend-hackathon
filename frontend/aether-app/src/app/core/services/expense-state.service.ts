import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ExpenseStateService {
  // A Subject to broadcast that an expense has been created or updated.
  private expenseMutatedSource = new Subject<void>();

  // Expose the subject as an observable for components to subscribe to.
  public expenseMutated$ = this.expenseMutatedSource.asObservable();

  /**
   * Components like the AddEditExpenseModal will call this method
   * after a successful save operation.
   */
  public notifyExpenseMutated(): void {
    // We don't need to pass any data, just the signal that something changed.
    this.expenseMutatedSource.next();
  }
}