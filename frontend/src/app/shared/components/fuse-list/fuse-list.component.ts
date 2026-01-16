import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, TemplateRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ColumnMenuComponent } from '../column-menu/column-menu.component';
import { SharedStateService } from 'app/shared/services/shared-state.service';

export interface ColumnDef {
  name: string;
  prop: string;
  visible: boolean;
  cellTemplate?: TemplateRef<any>;
}

@Component({
  standalone: true,
  selector: 'app-fuse-list',
  templateUrl: './fuse-list.component.html',
  styleUrls: ['./fuse-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.Default,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatCardModule,
    ColumnMenuComponent
  ]
})
export class FuseListComponent implements OnChanges, OnInit {
  // @Input() title = '';           // page/card title
  //@Input() data: any[] = [];
  @Input() columns: ColumnDef[] = [];
  //@Input() columns: { name: string; prop: string; visible: boolean; cellTemplate?: TemplateRef<any>; }[] = [];
  @Input() actions: { edit?: boolean; delete?: boolean; email?: boolean } = {};
  @Input() loading = false;
  @Input() emptyMessage = 'No data to display';
  @Input() pageIndex: number = 0; // Current page index from parent
  @Input() pageSize: number = 10; // Page size from parent
  @Input() totalItems: number = 0; // Total items for paginator

  @Output() editAction = new EventEmitter<number>();
  @Output() deleteAction = new EventEmitter<number>();
  @Output() emailAction = new EventEmitter<number>();
  @Output() pageChange = new EventEmitter<PageEvent>();
  @Input() actionsTemplate?: TemplateRef<any>;
  @Input() rowClassFn?: (row: any) => { [klass: string]: any };

  searchTerm = '';
  // INSTEAD, add a setter:
  private _data: any[] = [];
  dataSource = new MatTableDataSource<any>([]);
  displayedColumns: string[] = [];
  visibleColumns: { name: string; prop: string; visible: boolean; cellTemplate?: TemplateRef<any>; }[] = [];

  @Input()
  set data(v: any[]) {
    console.log('Data setter 🔥', v);
    this._data = v || [];
    this.dataSource.data = this._data;
  }
  get data() {
    return this._data;
  }

  constructor(private shared: SharedStateService) {}
  // keep your columns setter (or ngOnChanges for columns)
  // @Input()
  // set columns(cols: ColumnDef[]) {
  //   const all = cols || [];
  //   this.visibleColumns = all.filter(c => c.visible);
  //   this.displayedColumns = this.visibleColumns.map(c => c.prop)
  //     .concat((this.actions.edit||this.actions.delete||this.actions.email) ? ['actions'] : []);
  // }
  ngOnInit() {
    // Seed the service with our defaults on first load
     this.shared.setColumns(this.columns);

    // Subscribe to any changes
    this.shared.columns$.subscribe(cols => {
      this.columns = cols;
      this.ngOnChanges({ columns: true } as any);
    });
  }
  ngOnChanges(changes: SimpleChanges) {
    // Recompute visible/displayed columns whenever columns *or* actions change
    if (changes.columns || changes.actions) {
      const all = this.columns || [];
      // 1) Only the ones parent marked visible
      this.visibleColumns = all.filter(c => c.visible);
      // 2) Gather their prop names
      this.displayedColumns = this.visibleColumns
        .map(c => c.prop);
      // 3) If any action flag is true, tack on the 'actions' column
      if (this.actions.edit || this.actions.delete || this.actions.email) {
        this.displayedColumns.push('actions');
      }
    }

    // Repopulate dataSource when data changes
    if (changes.data) {
      this.dataSource.data = this.data;
      if (this.searchTerm) {
        this.applyFilter();
      }
    }
  }
  //   if (changes.columns || changes.actions) {
  //     this.visibleColumns = this.columns.filter(c => c.visible);

  //     this.displayedColumns = this.visibleColumns
  //       .map(c => c.prop)
  //       .concat(
  //         (this.actions.edit || this.actions.delete || this.actions.email)
  //           ? ['actions']
  //           : []
  //       );
  //   }
  // }
  /** Called when menu emits updated column-visibility */
  onColumnChange(cols: ColumnDef[]) {
    this.columns = cols;         // update our @Input-backed array
    this.ngOnChanges({ columns: true } as any);
  }
  applyFilter() {
    this.dataSource.filter = this.searchTerm.trim().toLowerCase();
    // update totalItems if you want client-side paging
    this.totalItems = this.dataSource.filteredData.length;
    this.pageIndex = 0;
    this.emitPage();
  }

  onPageChange(e: PageEvent) {
    this.pageIndex = e.pageIndex;
    this.pageSize = e.pageSize;
    this.emitPage(e);
  }

  private emitPage(event?: PageEvent) {
    console.log('Page change:', event);

    this.pageChange.emit(event ?? {
      pageIndex: this.pageIndex,
      pageSize: this.pageSize,
      length: this.totalItems
    });
  }

  onEdit(id: number) { this.editAction.emit(id); }
  onDelete(id: number) { this.deleteAction.emit(id); }
  onEmail(id: number) { 
    console.log('Email action triggered for ID:', id);
    this.emailAction.emit(id); }
}
