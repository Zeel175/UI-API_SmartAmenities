import { Component, OnInit } from '@angular/core';
import { UnitService } from '../unit.service';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { Unit } from 'app/model/unit';
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
    selector: 'unit-list',
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
export class UnitListComponent implements OnInit {
    unitData: Unit[] = [];
    filteredData: Unit[] = [];
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;
    loading = false;
    page = ApplicationPage.unit;
    permissions = PermissionType;
    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    unitColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'Unit Name', prop: 'unitName', visible: true },
        { name: 'Building', prop: 'buildingName', visible: true },
        { name: 'Floor', prop: 'floorName', visible: true },
        { name: 'Occupancy Status', prop: 'occupancyStatusName', visible: true },
        { name: 'Is Active', prop: 'isActive', visible: true }
    ];

    constructor(private unitService: UnitService,
        private router: Router,
        private route: ActivatedRoute,
        private shared: SharedStateService,
        private notificationService: ToastrService,
        private permissionService: PermissionService) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Unit (PER_UNIT) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Unit (PER_UNIT) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Unit (PER_UNIT) - Delete');
        this.shared.setColumns(this.unitColumns);
        this.getUnitData();
    }

    getUnitData() {
        this.loading = true;
        this.unitService.getUnits(this.pageIndex, this.pageSize).subscribe((res: any) => {
            this.loading = false;
            this.unitData = res.items || res;
            this.filteredData = this.unitData;
            this.totalItems = res.totalCount || this.unitData.length;
        }, () => {
            this.loading = false;
            this.notificationService.error('Failed to load unit data.');
        });
    }

    onPageChange(evt: PageEvent) {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getUnitData();
    }

    editUnit(id: number) {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteUnit(id: number) {
        if (confirm('Delete this unit?')) {
            this.unitService.deleteUnit(id).subscribe(() => {
                this.notificationService.success('Deleted successfully.');
                this.getUnitData();
            }, () => this.notificationService.error('Delete failed.'));
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.unitData;
            return;
        }
        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();
        this.filteredData = (this.unitData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}