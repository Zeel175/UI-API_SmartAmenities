import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
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
import { ResidentMaster } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { ResidentMasterService } from '../resident-master.service';
import { fuseAnimations } from '@fuse/animations';

@Component({
    selector: 'resident-master-list',
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    standalone: true,
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
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
export class ResidentMasterListComponent implements OnInit {
    residentData: ResidentMaster[] = [];
    filteredData: any[] = [];
    filterColumn = '';
    filterValue = '';
    page = ApplicationPage.residentMaster;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    residentColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'Parent Name', prop: 'parentName', visible: true },
        { name: 'Email', prop: 'email', visible: true },
        { name: 'Mobile', prop: 'mobile', visible: true },
        { name: 'Face', prop: 'faceStatus', visible: true },
        { name: 'Fingerprint', prop: 'fingerStatus', visible: true },
        { name: 'Card', prop: 'cardStatus', visible: true },
        { name: 'Family Members', prop: 'familyMemberCount', visible: true },
        { name: 'Is Active', prop: 'isActive', visible: true }
    ];

    constructor(
        private residentService: ResidentMasterService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Resident (PER_RESIDENT) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Resident (PER_RESIDENT) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Resident (PER_RESIDENT) - Delete');
        this.getResidentData();
    }

    private getResidentData(): void {
        this.loading = true;
        this.residentService.getResidents(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                const items = result.items || result;
                this.residentData = (items || []).map((resident: any) => ({
                    ...resident,
                    parentName: `${resident.parentFirstName || ''} ${resident.parentLastName || ''}`.trim(),
                    familyMemberCount: resident.familyMembers ? resident.familyMembers.length : 0,
                    faceStatus: this.getEnrollmentStatus(resident.faceId),
                    fingerStatus: this.getEnrollmentStatus(resident.fingerId),
                    cardStatus: this.getEnrollmentStatus(resident.cardId)
                }));
                this.totalItems = result.totalCount || this.residentData.length;
                this.applyFilters();
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load resident data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getResidentData();
    }

    editResident(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteResident(id: number): void {
        if (confirm('Delete this resident?')) {
            this.residentService.deleteResident(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getResidentData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    applyFilters(): void {
        this.filteredData = this.residentData;
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.residentData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.residentData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }

    private getEnrollmentStatus(value?: string): string {
        return value && value.toString().trim() !== '' ? 'Enrolled' : 'Not Enrolled';
    }
}
