import { Component, OnInit, OnDestroy, forwardRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CategoryService } from '../../../../core/services/category.service';
import { Category } from '../../../../core/models/category.model';


@Component({
  selector: 'app-category-multi-select',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './category-multi-select.component.html',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => CategoryMultiSelectComponent),
      multi: true
    }
  ]
})
export class CategoryMultiSelectComponent implements OnInit, OnDestroy, ControlValueAccessor {
  private categoryService = inject(CategoryService);

  allCategories: Category[] = [];
  selectedCategories: Category[] = [];
  isDropdownOpen = false;
  
  // CVA methods
  onChange: (value: Category[]) => void = () => {};
  onTouched: () => void = () => {};
  private sub!: Subscription;

  writeValue(value: Category[] | null): void {
    this.selectedCategories = value || [];
  }
  registerOnChange(fn: any): void { this.onChange = fn; }
  registerOnTouched(fn: any): void { this.onTouched = fn; }

  ngOnInit(): void {
    this.sub = this.categoryService.getMyCategories().subscribe(categories => {
      this.allCategories = categories;
    });
  }

  ngOnDestroy(): void {
    this.sub?.unsubscribe();
  }

  toggleDropdown(): void {
    this.isDropdownOpen = !this.isDropdownOpen;
    if (!this.isDropdownOpen) {
      this.onTouched();
    }
  }

  toggleCategory(category: Category): void {
    const index = this.selectedCategories.findIndex(c => c.id === category.id);
    if (index > -1) {
      // Remove from selected
      this.selectedCategories.splice(index, 1);
    } else {
      // Add to selected
      this.selectedCategories.push(category);
    }
    this.onChange(this.selectedCategories);
  }

  isSelected(category: Category): boolean {
    return this.selectedCategories.some(c => c.id === category.id);
  }

  removeCategory(categoryToRemove: Category, event: MouseEvent): void {
    event.stopPropagation(); // Prevent the dropdown from opening when clicking the 'x'
    this.selectedCategories = this.selectedCategories.filter(c => c.id !== categoryToRemove.id);
    this.onChange(this.selectedCategories);
    this.onTouched();
  }
}