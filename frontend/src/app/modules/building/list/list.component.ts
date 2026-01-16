import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { BuildingService } from '../building.service';
import { CommonModule } from '@angular/common';
import { fuseAnimations } from '@fuse/animations';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { ApplicationPage, PermissionType } from 'app/core';
import { ToastrService } from 'ngx-toastr';
import { PermissionService } from 'app/core/service/permission.service';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { Building } from 'app/model';

@Component({
    selector: 'building-list',
    providers: [BuildingService],
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
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
export class BuildingListComponent implements OnInit {
    buildingData: Building[] = [];
    filteredData: Building[] = [];
    filterColumn = '';
    filterValue = '';
    page = ApplicationPage.building;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    buildingColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'Building Name', prop: 'buildingName', visible: true },
        { name: 'Property Name', prop: 'propertyName', visible: true },
        { name: 'Is Active', prop: 'isActive', visible: true }
    ];

    constructor(
        private buildingService: BuildingService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute,
        private shared: SharedStateService
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Building (PER_BUILDING) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Building (PER_BUILDING) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Building (PER_BUILDING) - Delete');
        this.shared.setColumns(this.buildingColumns);
        this.getBuildingData();
    }

    private getBuildingData() {
        this.loading = true;
        this.buildingService.getBuilding(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                this.buildingData = result.items || result;
                this.totalItems = result.totalCount || this.buildingData.length;
                this.applyFilters();
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load building data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getBuildingData();
    }

    editBuilding(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteBuilding(id: number): void {
        if (confirm('Delete this building?')) {
            this.buildingService.deleteBuilding(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getBuildingData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    applyFilters(): void {
        this.filteredData = this.buildingData;
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        // If no filter, reset to full data
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.buildingData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.buildingData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
