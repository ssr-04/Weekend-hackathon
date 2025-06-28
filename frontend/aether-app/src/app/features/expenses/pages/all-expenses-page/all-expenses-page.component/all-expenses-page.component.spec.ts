import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllExpensesPageComponent } from './all-expenses-page.component';

describe('AllExpensesPageComponent', () => {
  let component: AllExpensesPageComponent;
  let fixture: ComponentFixture<AllExpensesPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllExpensesPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllExpensesPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
