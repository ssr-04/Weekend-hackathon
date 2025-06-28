import { Component, EventEmitter, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface SortOption {
  value: string;
  label: string;
}
export type SortOrder = 'Asc' | 'Desc';


@Component({
  selector: 'app-sort-control.component',
  imports: [CommonModule],
  templateUrl: './sort-control.component.html',
  styleUrl: './sort-control.component.css'
})

export class SortControlComponent {
  @Input() options: SortOption[] = [];
  @Output() sortChanged = new EventEmitter<{ sortBy: string, sortOrder: SortOrder }>();
  
  sortBy = signal('');
  sortOrder = signal<SortOrder>('Desc');
  
  ngOnInit() {
    if (this.options.length > 0) {
      this.sortBy.set(this.options[0].value);
    }
  }

  onSortByChange(value: string) {
    this.sortBy.set(value);
    this.emitChanges();
  }

  toggleSortOrder() {
    this.sortOrder.update(order => order === 'Asc' ? 'Desc' : 'Asc');
    this.emitChanges();
  }

  private emitChanges() {
    this.sortChanged.emit({ sortBy: this.sortBy(), sortOrder: this.sortOrder() });
  }

}
