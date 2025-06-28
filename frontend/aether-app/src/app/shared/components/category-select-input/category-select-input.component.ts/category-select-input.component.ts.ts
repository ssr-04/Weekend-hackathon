import { Component, OnInit, forwardRef, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { Category } from '../../../../core/models/category.model';
import { CategoryService } from '../../../../core/services/category.service';


@Component({
  selector: 'app-category-select-input.component.ts',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './category-select-input.component.ts.html',
  styleUrl: './category-select-input.component.ts.css'
})

export class CategorySelectInputComponent implements OnInit, ControlValueAccessor {
  private categoryService = inject(CategoryService);

  allCategories = signal<Category[]>([]);
  filteredCategories = signal<Category[]>([]);
  
  selectedCategoryName = signal('');
  isDropdownOpen = signal(false);
  
  private searchSubject = new Subject<string>();
  
  // CVA methods
  onChange: any = () => {};
  onTouched: any = () => {};

  writeValue(value: string): void {
    // This component's value is the category name string
    this.selectedCategoryName.set(value || '');
  }
  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  ngOnInit(): void {
    this.categoryService.getMyCategories().subscribe(categories => {
      this.allCategories.set(categories);
      this.filteredCategories.set(categories);
    });

    this.searchSubject.pipe(
      debounceTime(200),
      distinctUntilChanged()
    ).subscribe(searchValue => {
      this.filterCategories(searchValue);
    });
  }

  onSearch(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.selectedCategoryName.set(value); // Update the search term
    this.onChange(value); // Immediately update the form control value
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
    this.isDropdownOpen.set(false);
  }

  get showCreateOption(): boolean {
    const searchTerm = this.selectedCategoryName();
    if (!searchTerm) return false;
    // Show create option if the search term doesn't exactly match any existing category name
    return !this.allCategories().some(cat => cat.name.toLowerCase() === searchTerm.toLowerCase());
  }
}
