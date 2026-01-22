import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { AmenityMaster } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { AmenityMasterService } from '../amenity-master.service';

@Component({
    selector: 'amenity-master-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    standalone: true,
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
        ColumnFilterComponent
    ]
})
export class AmenityMasterListComponent implements OnInit {
    amenityData: AmenityMaster[] = [];
    filteredData: AmenityMaster[] = [];
    page = ApplicationPage.amenityMaster;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    amenityColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'Name', prop: 'name', visible: true },
        { name: 'Type', prop: 'type', visible: true },
        { name: 'Building', prop: 'buildingName', visible: true },
        { name: 'Floor', prop: 'floorName', visible: true },
        { name: 'Status', prop: 'status', visible: true },
        { name: 'Location', prop: 'location', visible: true },
        { name: 'Max Capacity', prop: 'maxCapacity', visible: false },
        { name: 'Slot Required', prop: 'bookingSlotRequired', visible: false },
        { name: 'Slot Duration (Min)', prop: 'slotDurationMinutes', visible: false },
        { name: 'Requires Approval', prop: 'requiresApproval', visible: false },
        { name: 'Allow Guests', prop: 'allowGuests', visible: false },
        { name: 'Chargeable', prop: 'isChargeable', visible: false },
        { name: 'Charge Type', prop: 'chargeType', visible: false },
        { name: 'Base Rate', prop: 'baseRate', visible: false },
        { name: 'Available Days', prop: 'availableDays', visible: false },
        { name: 'Open Time', prop: 'openTime', visible: false },
        { name: 'Close Time', prop: 'closeTime', visible: false }
    ];

    constructor(
        private amenityService: AmenityMasterService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Amenity (PER_AMENITY) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Amenity (PER_AMENITY) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Amenity (PER_AMENITY) - Delete');
        this.getAmenityData();
    }

    private getAmenityData(): void {
        this.loading = true;
        this.amenityService.getAmenities(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                this.amenityData = result.items || result;
                this.totalItems = result.totalCount || this.amenityData.length;
                this.filteredData = this.amenityData;
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load amenity data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getAmenityData();
    }

    editAmenity(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteAmenity(id: number): void {
        if (confirm('Delete this amenity?')) {
            this.amenityService.deleteAmenity(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getAmenityData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.amenityData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.amenityData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
