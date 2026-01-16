import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogModule } from '@angular/material/dialog';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { Zone } from 'app/model';
import { ZoneService } from '../zone.service';
import { SharedStateService } from 'app/shared/services/shared-state.service';

@Component({
    selector: 'zone-list',
    standalone: true,
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    imports: [
        CommonModule,
        MatTableModule,
        MatPaginatorModule,
        MatSortModule,
        RouterModule,
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
        ColumnFilterComponent
    ]
})
export class ZoneListComponent implements OnInit {
    zoneData: Zone[] = [];
    filteredData: Zone[] = [];
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;
    loading = false;

    page = ApplicationPage.zone;
    permissions = PermissionType;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    zoneColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'Zone Name', prop: 'zoneName', visible: true },
        { name: 'Building Name', prop: 'buildingName', visible: true },
        { name: 'Is Active', prop: 'isActive', visible: true }
    ];

    constructor(
        private zoneService: ZoneService,
        private router: Router,
        private route: ActivatedRoute,
        private shared: SharedStateService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Zone (PER_ZONE) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Zone (PER_ZONE) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Zone (PER_ZONE) - Delete');
        this.shared.setColumns(this.zoneColumns);
        this.getZoneData();
    }

    getZoneData(): void {
        this.loading = true;
        this.zoneService.getZones(this.pageIndex, this.pageSize).subscribe({
            next: (res: any) => {
                this.loading = false;
                this.zoneData = res.items ?? res ?? [];
                this.filteredData = this.zoneData;
                this.totalItems = res.totalCount ?? this.zoneData.length;
            },
            error: () => {
                this.loading = false;
                this.notificationService.error('Failed to load zone data.');
            }
        });
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getZoneData();
    }

    editZone(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteZone(id: number): void {
        if (confirm('Delete this zone?')) {
            this.zoneService.deleteZone(id).subscribe({
                next: () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getZoneData();
                },
                error: () => this.notificationService.error('Delete failed.')
            });
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.zoneData;
            return;
        }
        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.zoneData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
