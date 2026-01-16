import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
// import { ApplicationPage, PermissionType } from '@app-core';
import { ApplicationPage, PermissionType } from 'app/core';
import { Role } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { RoleService } from '../role.service';
import { PermissionService } from 'app/core/service/permission.service';
import { UserService } from '../../user/user.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, UntypedFormControl } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { ColumnDef, FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { SharedStateService } from 'app/shared/services/shared-state.service';

// @Component({
//     templateUrl: './list.component.html',
//     styleUrls: ['./list.component.scss']
// })
@Component({
    selector: 'role-list',
    providers: [RoleService],
    templateUrl: './list.component.html',
    styleUrls: ['./list.component.scss'],
    encapsulation: ViewEncapsulation.None,
    //changeDetection: ChangeDetectionStrategy.OnPush,
    animations: fuseAnimations,
    imports: [
        CommonModule,
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
        ColumnFilterComponent,
        FuseListComponent
    ]
})

export class RoleListComponent implements OnInit {
    roleData: Role[] = [];
    filteredData: Role[] = [];
    filterColumn: string = '';
    filterValue = '';
    page: string = ApplicationPage.role;
    permissions = PermissionType;
    isActive: boolean;
    error: string;
    loading: boolean;

    searchData: { [key: string]: any } = {
        isActive: false
    };


    // Pagination variables
    pageIndex: number = 1;
    pageSize: number = 10;
    totalItems: number = 0;

    IsAddPemission: boolean = false;
    IsEditPermission: boolean = false;
    IsDeletePermission: boolean = false;
    roleColumns: ColumnDef[] = [
    { name: 'Role', prop: 'name', visible: true }
    ];

    constructor(private roleService: RoleService, private notificationService: ToastrService,
        private permissionService: PermissionService, private router: Router,
        private route: ActivatedRoute,private shared: SharedStateService
    ) {

    }

   ngOnInit(): void {
        this.IsAddPemission = this.permissionService.hasPermission('Role (PER_ROLE) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Role (PER_ROLE) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Role (PER_ROLE) - Delete');
        this.shared.setColumns(this.roleColumns);
        this.getRoleData();
    }

    private getRoleData() {
        this.loading = true;

        this.roleService.getRole()
            .subscribe((result: any) => {
                this.roleData = result;
                this.totalItems = result.length; // Set total items based on the array length
                this.applyFilters();
                this.loading = false;

            }, (error) => {
                console.log(error);
                this.loading = false;

            });
    }

    removeRole(id: number) {
        const result = confirm(`Are you sure, you want to delete this role?`);
        if (result) {
            this.roleService.deleteRole(id)
                .subscribe(() => {
                    this.getRoleData();
                }, () => {
                    this.notificationService.error("Something went wrong.");
                });
        }
    }

    // activateToggleRole(role: Role, isActive: boolean) {
    //     const result = confirm(`Are you sure you want to ${isActive ? 'Activate' : 'Deactivate'} this role?`);
    //     if (result) {
    //         this.roleService.toggleActivate(role.id, isActive)
    //             .subscribe(() => {
    //                 this.getRoleData(); // Refresh data
    //                 this.notificationService.success(`Role ${isActive ? 'activated' : 'deactivated'} successfully.`);
    //             }, () => {
    //                 this.notificationService.error("Something went wrong.");
    //             });
    //     }
    // }

    removeRolePermission(id: number) {
        const result = confirm(`Are you sure, you want to delete this Weight Check?`);
        if (result) {
            this.roleService.deleteRole(id)
                .subscribe(() => {
                    this.getRoleData();
                }, () => {
                    this.notificationService.error("Something went wrong.");
                });
        }
    }


    updateSearch(search: { [key: string]: any }) {
        this.searchData = Object.assign({}, search);
    }

    isActiveRow(row) {
        return {
            'text-dark': !row.isActive
        };
    }



    editRole(id: any): void {
        console.log('Edit role with ID:', id);
        this.router.navigate(['edit', id], { relativeTo: this.route });
        //this.router.navigate(['..', 'edit', id], { relativeTo: this.route });
    }

    deleteRole(id: any): void {
        console.log('Delete role with ID:', id);
        const result = confirm(`Are you sure, you want to delete this role?`);
        if (result) {
            this.roleService.deleteRole(id)
                .subscribe(() => {
                    this.getRoleData();
                }, () => {
                    this.notificationService.error("Something went wrong.");
                });
        }
    }
    // Update the pageIndex and fetch data when user navigates through pages
    onPageChange(event: PageEvent): void {
        this.pageIndex = event.pageIndex + 1; // Convert 0-based index to 1-based
        this.pageSize = event.pageSize;
        this.paginateData(); // Update visible data
    }

    private paginateData(): void {
        const startIndex = (this.pageIndex - 1) * this.pageSize;
        const endIndex = startIndex + this.pageSize;
        this.roleData = this.roleData.slice(startIndex, endIndex);
    }
    onColumnFilter(event: { prop: string; value: string }) {
    this.filterColumn = event?.prop || '';
    this.filterValue = (event?.value ?? '').trim();
    this.applyFilters();
  }

  private applyFilters(): void {
    let data = [...this.roleData];
    if (this.filterColumn && this.filterValue) {
      const v = this.filterValue.toLowerCase();
      data = data.filter(x => `${(x as any)[this.filterColumn] ?? ''}`.toLowerCase().includes(v));
    }
    this.filteredData = data;
  }
}
