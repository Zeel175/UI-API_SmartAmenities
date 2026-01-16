import { CommonModule } from '@angular/common';
import { Component, TemplateRef, ViewChild, ViewEncapsulation } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { ApplicationPage, PermissionType } from 'app/core';
import { AuditLog } from 'app/model/auditLog';
import { ExcelDownloadComponent } from 'app/shared/components/excel-download/excel-download.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { AuditLogService } from '../audit-log.service';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';

@Component({
  selector: 'audit-log-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss'],
  encapsulation: ViewEncapsulation.None,
  animations: fuseAnimations,
  //standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    // Material (kept for parity with User List)
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    // Shared
    FuseListComponent,
    ExcelDownloadComponent,
    ColumnFilterComponent
  ],
})
export class AuditLogListComponent {
auditLogData: AuditLog[] = [];
filteredData: AuditLog[] = [];
    filterColumn: string = '';
    filterValue = '';
  page = ApplicationPage.auditLog;
  permissions = PermissionType;

  loading = false;
  pageIndex = 1;
  pageSize = 10;
  totalItems = 0;

  IsAddPemission = false;
  IsEditPermission = false;
  IsDeletePermission = false;

  @ViewChild('actionDateTemplate', { static: true })
  actionDateTemplate!: TemplateRef<any>;

    auditLogColumns = [
      { name: 'Page Name',      prop: 'pageName',      visible: true },
      { name: 'User ID',        prop: 'userId',        visible: true },
      { name: 'Action Type',    prop: 'actionType',    visible: true },
      { name: 'Field Name',     prop: 'fieldName',     visible: true },
      { name: 'Old Value',      prop: 'oldValue',      visible: true },
      { name: 'New Value',      prop: 'newValue',      visible: true },
      { name: 'Entity Name',    prop: 'entityName',    visible: true },
      { name: 'Entity ID',      prop: 'entityId',      visible: true },
      {
        name: 'Action DateTime',
        prop: 'actionDateTime',
        visible: true,
        cellTemplate: this.actionDateTemplate, // use date pipe in template
      },
    ];
  constructor(
    private auditLogService: AuditLogService,
    private permissionService: PermissionService,
    private toast: ToastrService,
    private shared: SharedStateService
  ) {}

  ngOnInit(): void {
    // permissions (kept same as old page)
    this.IsAddPemission = this.permissionService.hasPermission('AuditLog (PER_AUDITLOG) - Add');
    this.IsEditPermission = this.permissionService.hasPermission('AuditLog (PER_AUDITLOG) - Edit');
    this.IsDeletePermission = this.permissionService.hasPermission('AuditLog (PER_AUDITLOG) - Delete');
    this.shared.setColumns(this.auditLogColumns);

    this.getAuditLogData();
  }

  private getAuditLogData(): void {
    this.loading = true;
    this.auditLogService.getAuditLog(this.pageIndex, this.pageSize).subscribe({
      next: (result: any) => {
        this.loading = false;
        // IMPORTANT: keep the raw date/time; let the template format it
        this.auditLogData = result.items;
        this.totalItems = result.totalCount;
        this.applyFilters();
      },
      error: (err) => {
        this.loading = false;
        console.error(err);
        this.toast.error('Failed to load audit logs.');
      },
    });
  }

  onPageChange(event: PageEvent): void {
    // Fuse list gives zero-based index
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.getAuditLogData();
  }
  onColumnFilter(event: { prop: string; value: string }) {
    this.filterColumn = event?.prop || '';
    this.filterValue = (event?.value ?? '').trim();
    this.applyFilters();
  }

  private applyFilters(): void {
    let data = [...this.auditLogData];
    if (this.filterColumn && this.filterValue) {
      const v = this.filterValue.toLowerCase();
      data = data.filter(x => `${(x as any)[this.filterColumn] ?? ''}`.toLowerCase().includes(v));
    }
    this.filteredData = data;
  }
}
