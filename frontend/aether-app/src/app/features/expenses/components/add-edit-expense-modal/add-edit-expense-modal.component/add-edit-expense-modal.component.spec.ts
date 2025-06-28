import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddEditExpenseModalComponent } from './add-edit-expense-modal.component';

describe('AddEditExpenseModalComponent', () => {
  let component: AddEditExpenseModalComponent;
  let fixture: ComponentFixture<AddEditExpenseModalComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AddEditExpenseModalComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AddEditExpenseModalComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
