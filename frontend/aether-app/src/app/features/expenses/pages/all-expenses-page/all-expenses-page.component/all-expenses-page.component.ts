import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule, DatePipe, formatDate } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, tap } from 'rxjs/operators';
import { DateFormatHelper } from '../../../../../core/helpers/date-format.helper';
import { Category } from '../../../../../core/models/category.model';
import { Expense } from '../../../../../core/models/expense.model';
import { ExpenseStateService } from '../../../../../core/services/expense-state.service';
import { ExpenseService } from '../../../../../core/services/expense.service';
import { ModalService } from '../../../../../core/services/modal.service';
import { CategoryMultiSelectComponent } from '../../../../../shared/components/category-multi-select/category-multi-select.component/category-multi-select.component';
import { ExpenseListItemComponent } from '../../../../../shared/components/expense-list-item/expense-list-item.component/expense-list-item.component';
import { SortControlComponent, SortOption } from '../../../../../shared/components/sort-control/sort-control.component/sort-control.component';


// Interface for our grouped expenses
export interface GroupedExpenses {
  date: string;
  expenses: Expense[];
}

@Component({
  selector: 'app-all-expenses-page',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    DatePipe,
    ExpenseListItemComponent,
    CategoryMultiSelectComponent,
    SortControlComponent,
  ],
  templateUrl: './all-expenses-page.component.html',
})
export class AllExpensesPageComponent implements OnInit, OnDestroy {
  private expenseService = inject(ExpenseService);
  private expenseStateService = inject(ExpenseStateService);
  private modalService = inject(ModalService);
  private fb = inject(FormBuilder);

  private filterSub!: Subscription;
  private expenseMutatedSub!: Subscription;

  allExpenses: Expense[] = [];
  groupedExpenses = signal<GroupedExpenses[]>([]);
  
  isLoading = signal(true);
  currentPage = 1;
  totalPages = 1;

  filterForm: FormGroup;
  sortOptions: SortOption[] = [
    { value: 'date', label: 'Date' },
    { value: 'amount', label: 'Amount' }
  ];

  constructor() {
    this.filterForm = this.fb.group({
      SearchTerm: [''],
      CategoryNames: [[]],
      PaymentMethod: [''],
      StartDate: [''],
      EndDate: [''],
      SortBy: ['date'],
      SortOrder: ['Desc']
    });
  }

  ngOnInit(): void {
    this.filterSub = this.filterForm.valueChanges.pipe(
      debounceTime(400),
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
      tap(() => {
        this.currentPage = 1; // Reset pagination on any filter change
        this.allExpenses = []; // Clear previous results
        this.isLoading.set(true);
      }),
      switchMap(filters => this.fetchExpenses(1, filters))
    ).subscribe(this.handleExpenseResponse.bind(this));

    // Initial fetch
    this.filterForm.updateValueAndValidity();

    // Listen for mutations from the Add/Edit modal to refresh the list
    this.expenseMutatedSub = this.expenseStateService.expenseMutated$.subscribe(() => {
      this.filterForm.updateValueAndValidity();
    });
  }
  
  ngOnDestroy(): void {
    this.filterSub?.unsubscribe();
    this.expenseMutatedSub?.unsubscribe();
  }

  fetchExpenses(page: number, filters: any) {
    const apiFilters = {
      ...filters,
      PageNumber: page,
      PageSize: 20,
      // Format data for the API
      CategoryNames: filters.CategoryNames?.map((c: Category) => c.name).join(','),
      StartDate: filters.StartDate ? DateFormatHelper.formatDateForApi(filters.StartDate) : null,
      EndDate: filters.EndDate ? DateFormatHelper.formatDateForApi(filters.EndDate, true) : null,
    };
    return this.expenseService.getMyExpenses(apiFilters);
  }

  handleExpenseResponse(response: any): void {
    this.allExpenses = this.currentPage === 1 ? response.items : [...this.allExpenses, ...response.items];
    this.totalPages = response.totalPages;
    this.groupExpensesByDate();
    this.isLoading.set(false);
  }

  loadMore(): void {
    if (this.currentPage >= this.totalPages) return;
    this.currentPage++;
    this.isLoading.set(true);
    this.fetchExpenses(this.currentPage, this.filterForm.value)
        .subscribe(this.handleExpenseResponse.bind(this));
  }

  groupExpensesByDate(): void {
    const groups = this.allExpenses.reduce((acc, expense) => {
      const dateKey = formatDate(expense.date, 'yyyy-MM-dd', 'en-US');
      if (!acc[dateKey]) {
        acc[dateKey] = [];
      }
      acc[dateKey].push(expense);
      return acc;
    }, {} as { [key: string]: Expense[] });
    
    this.groupedExpenses.set(
      Object.keys(groups).map(date => ({ date, expenses: groups[date] }))
    );
  }

  onSortChanged(sort: { sortBy: string, sortOrder: string }) {
    this.filterForm.patchValue(sort);
  }

  onEditExpense(expense: Expense): void {
    this.modalService.open({ type: 'addEditExpense', expense: expense });
  }
}