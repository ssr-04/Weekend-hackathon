import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule, CurrencyPipe, DatePipe } from '@angular/common';
import { trigger, transition, style, animate } from '@angular/animations';
import { Expense } from '../../../../core/models/expense.model';

@Component({
  selector: 'app-expense-list-item',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, DatePipe],
  templateUrl: './expense-list-item.component.html',
  animations: [
    trigger('fadeInOut', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(-10px)' }),
        animate('300ms ease-out', style({ opacity: 1, transform: 'translateY(0)' })),
      ]),
      transition(':leave', [
        animate('300ms ease-in', style({ opacity: 0, transform: 'scale(0.9)' }))
      ]),
    ]),
  ],
})
export class ExpenseListItemComponent {
  @Input() expense!: Expense;
  @Output() edit = new EventEmitter<Expense>();
  @Output() delete = new EventEmitter<string>(); // Emits the expense ID

  onEdit(): void {
    this.edit.emit(this.expense);
  }

  onDelete(): void {
    if (confirm(`Are you sure you want to delete the expense: "${this.expense.title}"?`)) {
      this.delete.emit(this.expense.id);
    }
  }
}