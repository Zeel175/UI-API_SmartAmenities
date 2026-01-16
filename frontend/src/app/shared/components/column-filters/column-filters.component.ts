import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
// import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common'; // <-- Add this import
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
@Component({
  selector: 'app-column-filters',
  templateUrl: './column-filters.component.html',
  styleUrls: ['./column-filters.component.scss'],
  standalone: true,
  imports: [
    CommonModule, MatInputModule, MatFormFieldModule, MatSelectModule,
    FormsModule
  ]
})
export class ColumnFiltersComponent {
  @Input() columns: any[] = []; // Available columns
  @Input() data: any[] = []; // Original dataset

  @Output() filterChange = new EventEmitter<{
    prop: string;
    value: string;
    operator: 'contains' | 'equals' | 'startsWith' | 'endsWith'
  }>();
  //@Output() filterChange = new EventEmitter<{ prop: string; value: string }>();

  selectedColumn: string = '';
  //selectedOperator: string = 'contains'; // Default operator
  selectedOperator: 'contains' | 'equals' | 'startsWith' | 'endsWith' = 'contains';
  filterValue: string = '';

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
      this.filterChange.emit({ prop: '', value: '', operator: this.selectedOperator });
      return;
    }

    // otherwise emit the real filter
    this.filterChange.emit({
      prop: this.selectedColumn,
      value: this.filterValue.trim().toLowerCase(),
      operator: this.selectedOperator
    });
  }

}
