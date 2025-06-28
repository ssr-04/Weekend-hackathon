import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LandingFeatureCardComponent } from './landing-feature-card.component';

describe('LandingFeatureCardComponent', () => {
  let component: LandingFeatureCardComponent;
  let fixture: ComponentFixture<LandingFeatureCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LandingFeatureCardComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LandingFeatureCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
