import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-stat-card',
  imports: [CommonModule],
  templateUrl: './stat-card.component.html',
  styleUrl: './stat-card.component.css'
})

export class StatCardComponent {
  /** The main label for the statistic (e.g., "Total Expenses") */
  @Input() label: string = 'Statistic';
  
  /** The large, primary value to be displayed. */
  @Input() value: string | number | null = 0;
  
  /** Optional smaller text displayed below the value. */
  @Input() subtext?: string | null;
  
  /** Toggles the skeleton loading state. */
  @Input() isLoading: boolean = false;
}
