// excel-download.component.ts
import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule }     from '@angular/common';
import { MatButtonModule }  from '@angular/material/button';
import { MatIconModule }    from '@angular/material/icon';
import * as XLSX from 'xlsx';

@Component({
  standalone: true,
  selector: 'app-excel-download',
  template: `
    <button
      mat-flat-button
      color="primary"
      class="inline-flex items-center whitespace-nowrap"
      (click)="downloadExcel()"
    >
      <mat-icon>download</mat-icon>
      <span class="ml-2 font-medium">Export to Excel</span>
    </button>
  `,
  styles: [`
    :host ::ng-deep button.mat-flat-button {
      padding: 0.5rem 1rem;
    }
  `],
  changeDetection: ChangeDetectionStrategy.Default,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule
  ]
})
export class ExcelDownloadComponent {
  @Input() data: any[] = [];
  @Input() columns: { name: string; prop: string; visible: boolean }[] = [];

  downloadExcel() {
    const visibleCols = this.columns.filter(c => c.visible);
    const headers    = visibleCols.map(c => c.name);
    const props      = visibleCols.map(c => c.prop);

    const filtered = this.data.map(row =>
      props.reduce((acc, p) => (acc[p] = row[p], acc), {} as any)
    );

    const ws = XLSX.utils.json_to_sheet(filtered);
    XLSX.utils.sheet_add_aoa(ws, [headers], { origin: 'A1' });
    const wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, 'Sheet1');
    XLSX.writeFile(wb, 'ExportedData.xlsx');
  }
}
