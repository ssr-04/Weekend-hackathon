import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LandingFeatureCardComponent } from '../../../components/landing-feature-card/landing-feature-card.component/landing-feature-card.component';

@Component({
  selector: 'app-landing-page',
  standalone: true,
  imports: [LandingFeatureCardComponent, RouterLink],
  templateUrl: './landing-page.component.html',
})
export class LandingPageComponent { }