import { Component, OnInit } from '@angular/core';
import { FloorService } from '../floor.service';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { Floor } from 'app/model';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';

@Component({
    selector: 'floor-list',
    standalone: true,
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    imports: [CommonModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule, RouterModule,
        MatIconModule,
        MatButtonModule,
        MatCheckboxModule,
        MatFormFieldModule,
        MatInputModule,
        MatCardModule,
        FormsModule,
        ReactiveFormsModule,
        FuseListComponent,
        MatDialogModule,
         ColumnFilterComponent]
})
export class FloorListComponent implements OnInit {
    floorData: Floor[] = [];
   // displayedColumns = ['code', 'floorName', 'buildingName', 'actions'];
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;
    loading = false;
    page = ApplicationPage.floor;
    permissions = PermissionType;
    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

  floorColumns = [
  { name: 'Code', prop: 'code', visible: true },
  { name: 'Floor Name', prop: 'floorName', visible: true },
  { name: 'Building', prop: 'buildingName', visible: true }
];

    filteredData: Floor[] = []; // ✅ add this line

    constructor(private floorService: FloorService, private router: Router, private route: ActivatedRoute, private shared: SharedStateService, private notificationService: ToastrService, private permissionService: PermissionService) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Floor (PER_FLOOR) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Floor (PER_FLOOR) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Floor (PER_FLOOR) - Delete');
        this.shared.setColumns(this.floorColumns);
        this.getFloorData();
    }

    getFloorData() {
        this.floorService.getFloor(this.pageIndex, this.pageSize).subscribe((res: any) => {
            this.floorData = res.items;
            this.filteredData = res.items;   // ✅ assign here
            this.totalItems = res.totalCount;
        });
    }

    onPageChange(evt: PageEvent) {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getFloorData();
    }

    editFloor(id: number) {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteFloor(id: number) {
        if (confirm('Delete this floor?')) {
            this.floorService.deleteFloor(id).subscribe(() => {
                this.notificationService.success('Deleted successfully.');
                this.getFloorData();
            }, () => this.notificationService.error('Delete failed.'));
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.floorData;
            return;
        }
        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();
        this.filteredData = (this.floorData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
