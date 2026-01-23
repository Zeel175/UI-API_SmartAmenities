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
import { AmenityUnit } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { AmenityUnitMasterService } from '../amenity-unit-master.service';

@Component({
    selector: 'amenity-unit-master-list',
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
export class AmenityUnitMasterListComponent implements OnInit {
    amenityUnitData: AmenityUnit[] = [];
    filteredData: AmenityUnit[] = [];
    page = ApplicationPage.amenityUnitMaster;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    amenityUnitColumns = [
        { name: 'Amenity', prop: 'amenityName', visible: true },
        { name: 'Unit Name', prop: 'unitName', visible: true },
        { name: 'Unit Code', prop: 'unitCode', visible: true },
        { name: 'Status', prop: 'status', visible: true },
        { name: 'Short Description', prop: 'shortDescription', visible: true },
        { name: 'Chargeable', prop: 'isChargeable', visible: false },
        { name: 'Charge Type', prop: 'chargeType', visible: false },
        { name: 'Base Rate', prop: 'baseRate', visible: false },
        { name: 'Tax Applicable', prop: 'taxApplicable', visible: false }
    ];

    constructor(
        private amenityUnitService: AmenityUnitMasterService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Amenity Unit (PER_AMENITY_UNIT) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Amenity Unit (PER_AMENITY_UNIT) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Amenity Unit (PER_AMENITY_UNIT) - Delete');
        this.getAmenityUnitData();
    }

    private getAmenityUnitData(): void {
        this.loading = true;
        this.amenityUnitService.getAmenityUnits(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                this.amenityUnitData = result.items || result;
                this.totalItems = result.totalCount || this.amenityUnitData.length;
                this.filteredData = this.amenityUnitData;
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load amenity units.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getAmenityUnitData();
    }

    editAmenityUnit(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteAmenityUnit(id: number): void {
        if (confirm('Delete this amenity unit?')) {
            this.amenityUnitService.deleteAmenityUnit(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getAmenityUnitData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.amenityUnitData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.amenityUnitData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
