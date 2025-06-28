import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { DateFormatHelper } from '../../../../core/helpers/date-format.helper';

@Component({
  selector: 'app-date-range-picker',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './date-range-picker.component.html',
  styleUrl: './date-range-picker.component.css'
})

export class DateRangePickerComponent {
  @Output() rangeSelected = new EventEmitter<{ startDate?: string; endDate?: string }>();

  rangeForm = new FormBuilder().group({
    startDate: [''],
    endDate: ['']
  });

  setPreset(preset: 'today' | 'this_week' | 'this_month'): void {
    const today = new Date();
    let startDate = new Date();
    
    if (preset === 'today') {
      startDate = today;
    } else if (preset === 'this_week') {
      startDate.setDate(today.getDate() - today.getDay());
    } else if (preset === 'this_month') {
      startDate = new Date(today.getFullYear(), today.getMonth(), 1);
    }
    
    const formattedStart = DateFormatHelper.formatDateForApi(startDate);
    const formattedEnd = DateFormatHelper.formatDateForApi(today, true);

    this.rangeForm.patchValue({ startDate: '', endDate: '' }); // Clear manual inputs
    this.rangeSelected.emit({ startDate: formattedStart, endDate: formattedEnd });
  }

  applyManualRange(): void {
    const { startDate, endDate } = this.rangeForm.value;
    if (startDate && endDate) {
      this.rangeSelected.emit({
        startDate: DateFormatHelper.formatDateForApi(startDate),
        endDate: DateFormatHelper.formatDateForApi(endDate, true)
      });
    }
  }
}

