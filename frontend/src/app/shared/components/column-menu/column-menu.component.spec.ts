import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ColumnMenuComponent } from './column-menu.component';

describe('ColumnMenuComponent', () => {
  let component: ColumnMenuComponent;
  let fixture: ComponentFixture<ColumnMenuComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ColumnMenuComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ColumnMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
