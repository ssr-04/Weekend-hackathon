import { Component, OnInit, forwardRef, inject, signal, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
// CORRECTED: Import NG_VALUE_ACCESSOR to use in the provider
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

import { Subject, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Category } from '../../../../core/models/category.model';
import { CategoryService } from '../../../../core/services/category.service';

@Component({
  selector: 'app-category-select-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-select-input.component.ts.html',
  styleUrls: [],
  // --- THE FIX ---
  // This providers array correctly registers the component as a ControlValueAccessor,
  // allowing it to work with formControlName.
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CategorySelectInputComponent),
      multi: true
    }
  ]
  // ---------------
})
export class CategorySelectInputComponent implements OnInit, OnDestroy, ControlValueAccessor {
  private categoryService = inject(CategoryService);

  allCategories = signal<Category[]>([]);
  filteredCategories = signal<Category[]>([]);
  
  selectedCategoryName = signal('');
  isDropdownOpen = signal(false);
  
  private searchSubject = new Subject<string>();
  private searchSubscription!: Subscription;
  
  // CVA methods
  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(value: string): void {
    this.selectedCategoryName.set(value || '');
  }
  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  ngOnInit(): void {
    this.categoryService.getMyCategories().subscribe(categories => {
      this.allCategories.set(categories);
      this.filteredCategories.set(categories);
    });

    this.searchSubscription = this.searchSubject.pipe(
      debounceTime(200),
      distinctUntilChanged()
    ).subscribe(searchValue => {
      this.filterCategories(searchValue);
    });
  }

  ngOnDestroy(): void {
    if (this.searchSubscription) {
      this.searchSubscription.unsubscribe();
    }
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.selectedCategoryName.set(value);
    this.onChange(value);
    this.onTouched(); // Mark as touched on input
    this.searchSubject.next(value);
    this.isDropdownOpen.set(true);
  }

  filterCategories(search: string): void {
    if (!search) {
      this.filteredCategories.set(this.allCategories());
      return;
    }
    const lowerCaseSearch = search.toLowerCase();
    this.filteredCategories.set(
      this.allCategories().filter(cat => cat.name.toLowerCase().includes(lowerCaseSearch))
    );
  }

  selectCategory(categoryName: string): void {
    this.selectedCategoryName.set(categoryName);
    this.onChange(categoryName);
    this.onTouched();
    this.isDropdownOpen.set(false);
  }

  get showCreateOption(): boolean {
    const searchTerm = this.selectedCategoryName();
    if (!searchTerm) return false;
    return !this.allCategories().some(cat => cat.name.toLowerCase() === searchTerm.toLowerCase());
  }
}