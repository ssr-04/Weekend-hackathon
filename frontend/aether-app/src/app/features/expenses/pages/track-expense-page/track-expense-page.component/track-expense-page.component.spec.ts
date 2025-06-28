import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackExpensePageComponent } from './track-expense-page.component';

describe('TrackExpensePageComponent', () => {
  let component: TrackExpensePageComponent;
  let fixture: ComponentFixture<TrackExpensePageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackExpensePageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackExpensePageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
