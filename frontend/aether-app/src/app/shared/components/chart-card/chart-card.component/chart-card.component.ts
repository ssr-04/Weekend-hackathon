import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chart-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chart-card.component.html',
})
export class ChartCardComponent {
  @Input() title: string = 'Chart';
  @Input() isLoading: boolean = false;
  // Allows us to set a specific height for skeleton loading to prevent layout shifts
  @Input() cardHeightClass: string = 'h-98'; 
}