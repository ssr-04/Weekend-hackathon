import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CategorySelectInputComponentTs } from './category-select-input.component.ts';

describe('CategorySelectInputComponentTs', () => {
  let component: CategorySelectInputComponentTs;
  let fixture: ComponentFixture<CategorySelectInputComponentTs>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CategorySelectInputComponentTs]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CategorySelectInputComponentTs);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
