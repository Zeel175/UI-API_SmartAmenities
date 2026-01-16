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
import { GuestMaster } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { GuestMasterService } from '../guest-master.service';
import { fuseAnimations } from '@fuse/animations';

@Component({
    selector: 'guest-master-list',
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
export class GuestMasterListComponent implements OnInit {
    guestData: GuestMaster[] = [];
    filteredData: any[] = [];
    filterColumn = '';
    filterValue = '';
    page = ApplicationPage.guestMaster;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    guestColumns = [
        { name: 'Code', prop: 'code', visible: true },
        { name: 'First Name', prop: 'firstName', visible: true },
        { name: 'Last Name', prop: 'lastName', visible: true },
        { name: 'Email', prop: 'email', visible: true },
        { name: 'Mobile', prop: 'mobile', visible: true },
        { name: 'Is Active', prop: 'isActive', visible: true }
    ];

    constructor(
        private guestService: GuestMasterService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Guest (PER_GUEST) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Guest (PER_GUEST) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Guest (PER_GUEST) - Delete');
        this.getGuestData();
    }

    private getGuestData(): void {
        this.loading = true;
        this.guestService.getGuests(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                const items = result.items || result;
                this.guestData = (items || []).map((guest: any) => ({
                    ...guest,
                    parentName: `${guest.parentFirstName || ''} ${guest.parentLastName || ''}`.trim(),
                    familyMemberCount: guest.familyMembers ? guest.familyMembers.length : 0
                }));
                this.totalItems = result.totalCount || this.guestData.length;
                this.applyFilters();
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load guest data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getGuestData();
    }

    editGuest(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteGuest(id: number): void {
        if (confirm('Delete this guest?')) {
            this.guestService.deleteGuest(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getGuestData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    applyFilters(): void {
        this.filteredData = this.guestData;
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.guestData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.guestData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
