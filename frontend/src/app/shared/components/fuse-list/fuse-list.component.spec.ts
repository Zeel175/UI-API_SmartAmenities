import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FuseListComponent } from './fuse-list.component';

describe('FuseListComponent', () => {
  let component: FuseListComponent;
  let fixture: ComponentFixture<FuseListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FuseListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FuseListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
