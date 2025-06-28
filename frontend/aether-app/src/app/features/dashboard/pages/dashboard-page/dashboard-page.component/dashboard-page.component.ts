import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { CommonModule, CurrencyPipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { NgxChartsModule, Color, ScaleType, LegendPosition } from '@swimlane/ngx-charts';
import { Subscription, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap } from 'rxjs/operators';
import { DateFormatHelper } from '../../../../../core/helpers/date-format.helper';
import { UserProfile } from '../../../../../core/models/user.model';
import { UserService } from '../../../../../core/services/user.service';
import { ChartCardComponent } from '../../../../../shared/components/chart-card/chart-card.component/chart-card.component';
import { DateRangePickerComponent } from '../../../../../shared/components/date-range-picker/date-range-picker.component/date-range-picker.component';
import { StatCardComponent } from '../../../../../shared/components/stat-card/stat-card.component/stat-card.component';
import { DashboardDataService, DashboardFilters } from '../../../dashboard-data.service';


// Interface for form typing
interface DashboardFilterForm {
  dateRange: {
    startDate?: string | null;
    endDate?: string | null;
  };
}

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    CurrencyPipe,
    ReactiveFormsModule,
    NgxChartsModule,
    DateRangePickerComponent,
    ChartCardComponent,
    StatCardComponent
  ],
  templateUrl: './dashboard-page.component.html',
  // Provide the service here to create a new instance for this component and its children
  providers: [DashboardDataService], 
})
export class DashboardPageComponent implements OnInit, OnDestroy {
  // --- Injections ---
  dataService = inject(DashboardDataService);
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  
  private filterSub!: Subscription;
  
  // --- Component State ---
  userProfile: UserProfile | null = null;
  filterForm: FormGroup;

  // --- Chart Styling ---
  legendPosition = LegendPosition.Below;
  chartColorSchema: Color = {
    name: 'aether',
    selectable: true,
    group: ScaleType.Ordinal,
    domain: ['#5eead4', '#38bdf8', '#8b5cf6', '#c084fc', '#f472b6', '#f87171', '#facc15']
  };

  constructor() {
    this.filterForm = this.fb.group({
      dateRange: this.fb.group({
        startDate: [''],
        endDate: ['']
      })
    });
  }
  
  ngOnInit(): void {
    // Fetch the user profile for the static "Income" card
    this.userService.getMyProfile().subscribe(profile => {
      this.userProfile = profile;
    });

    // Set a default filter range on initial load
    this.setDefaultDateRange();

    // Listen to filter changes and tell the service to re-fetch data
    this.filterSub = this.filterForm.valueChanges.pipe(
      debounceTime(50),
      distinctUntilChanged((prev, curr) => JSON.stringify(prev) === JSON.stringify(curr)),
      switchMap((formValue: DashboardFilterForm) => {
        const filters: DashboardFilters = {
          startDate: formValue.dateRange?.startDate || undefined,
          endDate: formValue.dateRange?.endDate || undefined
        };
        // The component's ONLY job is to tell the service to fetch.
        this.dataService.fetchDashboardData(filters);
        return of(null);
      })
    ).subscribe();
  }

  ngOnDestroy(): void {
    if (this.filterSub) {
      this.filterSub.unsubscribe();
    }
  }

  onDateRangeSelected(range: { startDate?: string; endDate?: string }): void {
    this.filterForm.get('dateRange')?.setValue(range);
  }

  private setDefaultDateRange(): void {
    const today = new Date();
    const startDate = new Date(today.getFullYear(), today.getMonth(), 1);

    // Use patchValue to trigger the valueChanges subscription
    this.filterForm.get('dateRange')?.patchValue({
      startDate: DateFormatHelper.formatDateForApi(startDate),
      endDate: DateFormatHelper.formatDateForApi(today, true)
    });
  }
}