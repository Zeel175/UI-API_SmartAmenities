import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ExcelDownloadComponent } from './excel-download.component';

describe('ExcelDownloadComponent', () => {
  let component: ExcelDownloadComponent;
  let fixture: ComponentFixture<ExcelDownloadComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ExcelDownloadComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(ExcelDownloadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
