import { Component, Input } from '@angular/core';
import { CommonModule, NgClass } from '@angular/common';

@Component({
  selector: 'app-landing-feature-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './landing-feature-card.component.html',
})
export class LandingFeatureCardComponent {
  @Input() title: string = '';
  @Input() description: string = '';
  @Input() icon: 'chart' | 'ai' | 'track' = 'track';
  @Input() alignment: 'left' | 'right' = 'left';
}