import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
// import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // <-- Add this import
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-column-filter',
  templateUrl: './column-filter.component.html',
  styleUrls: ['./column-filter.component.scss'],
  standalone: true,
  imports: [
    CommonModule, MatInputModule, MatFormFieldModule, MatSelectModule,
    FormsModule
  ]
})
export class ColumnFilterComponent {
  @Input() columns: any[] = []; // Available columns
  @Input() data: any[] = []; // Original dataset

  //@Output() filterChange = new EventEmitter<any[]>(); // Filtered data event

  @Output() filterChange = new EventEmitter<{ prop: string; value: string }>();

  selectedColumn: string = '';
  selectedOperator: string = 'contains'; // Default operator
  filterValue: string = '';
  // ngOnInit() {
  //   console.log('ColumnFilterComponent columns:', this.columns);
  // }
  ngOnChanges(changes: SimpleChanges) {
    if (changes.columns) {
      console.log('ColumnFilterComponent columns changed:', changes.columns.currentValue);
    }
  }
  // applyFilter() {
  //   if (!this.selectedColumn || !this.filterValue) {
  //     //this.filterChange.emit(this.data);
  //     this.filterChange.emit({ prop: '', value: '' });
  //     return;
  //   }

    applyFilter() {
    const v = this.filterValue.trim().toLowerCase();

    // if either column OR value is blank, tell the parent to clear its filter
    if (!this.selectedColumn || !v) {
      this.filterChange.emit({ prop: '', value: '' });
      return;
    }

    // otherwise emit the real filter
    this.filterChange.emit({
      prop: this.selectedColumn,
      value: v
    });
  }

}
