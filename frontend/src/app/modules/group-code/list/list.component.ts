import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, ChangeDetectorRef, Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormsModule, ReactiveFormsModule, UntypedFormControl } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { RouterModule } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { ApplicationPage } from 'app/core';
import { GroupCode } from 'app/model';
import { RoleService } from 'app/modules/role/role.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { GroupCodeService } from '../group-code.service';
import { ToastrService } from 'ngx-toastr';
import { PermissionService } from 'app/core/service/permission.service';
import { debounceTime, distinctUntilChanged } from 'rxjs';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { ExcelDownloadComponent } from 'app/shared/components/excel-download/excel-download.component';
import { EmailSenderComponent } from 'app/shared/components/email-sender/email-sender.component';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressBarModule } from '@angular/material/progress-bar';

type PagedResponse<T> = {
    items: T[];
    totalCount: number;
    pageIndex?: number;
    pageSize?: number;
};

@Component({
    selector: 'group-code-list',
    providers: [GroupCodeService],
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
       // ColumnFilterComponent,
        FuseListComponent,
   // ExcelDownloadComponent,
   // EmailSenderComponent,
    MatDialogModule,
    ColumnFilterComponent,
    MatProgressBarModule
    ]
})
export class GroupCodeListComponent implements OnInit {


    @ViewChild(MatPaginator) paginator: MatPaginator;
    @ViewChild(MatSort) sort: MatSort;
    groupCodeData: GroupCode[] = [];
    filteredData: GroupCode[] = [];
    columns = [
  { name: 'Code',       prop: 'code',      visible: true },
  { name: 'Name',       prop: 'name',      visible: true },
  { name: 'Group Name', prop: 'groupName', visible: true },
  { name: 'Priority',   prop: 'priority',  visible: true },
  { name: 'Value',      prop: 'value',     visible: true }
];
    page: string = ApplicationPage.group_code;
    loading = false;
    searchInputControl: UntypedFormControl = new UntypedFormControl('');
    showAllGroupCodesControl: UntypedFormControl = new UntypedFormControl(false);
    pageIndex = 0;
    pageSize = 10;
    totalItems = 0;
    filterColumn: string = '';
    globalSearch: string = '';
    filterValue = '';
    searchData: { [key: string]: any } = {
        isActive: false
    };
    hasViewPermission = false;
    hasAddPermission = false;
    hasEditPermission = false;
    hasDeletePermission = false;


    displayedColumns: string[] = [
        'code',
        'name',
        'groupName',
        'priority',
        'value',
        'actions',
    ];

    constructor(
        private groupCodeService: GroupCodeService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private _cdr: ChangeDetectorRef
    ) { }
    editGroupCode = (id: number) => {
    // navigate just like your current [routerLink] does
    // (if you prefer, inject Router and do this.router.navigate(['/group-code/edit', id]))
    window.location.href = `/group-code/edit/${id}`;
    };
    
    ngOnInit(): void {
        this.hasViewPermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - View');
        this.hasEditPermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - Edit');
        this.hasAddPermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - Add');
        this.hasDeletePermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - Delete');


        // “Show All” now filters locally
        this.showAllGroupCodesControl.valueChanges.subscribe(_ => {
            this.pageIndex = 0;
            this.applyFilters();
        });

        // Global search now filters locally
        this.searchInputControl.valueChanges
            .pipe(debounceTime(300), distinctUntilChanged())
            .subscribe(text => {
                this.globalSearch = (text ?? '').trim();
                this.pageIndex = 0;
                this.applyFilters();
            });

        // Initial load (page 1 on server)
        this.getGroupCodes();
    }

    private getGroupCodes(): void {
        this.loading = true;

        // Server expects 1-based page index; UI is 0-based
        const serverPage = this.pageIndex + 1;

        this.groupCodeService.getGroupCodes(serverPage, this.pageSize).subscribe({
            next: (resp: PagedResponse<GroupCode> | any) => {
                // resp is the PaginatedList<T> from backend
                this.groupCodeData = resp?.items ?? [];
                this.totalItems = resp?.totalCount ?? 0;

                this.applyFilters();  // filter current page locally
                this.loading = false;
                this._cdr.markForCheck();
            },
            error: (err) => {
                this.groupCodeData = [];
                this.filteredData = [];
                this.totalItems = 0;
                this.loading = false;
                this._cdr.markForCheck();
                this.notificationService.error('Error fetching data.');
                console.error('API Error:', err);
            }
        });
    }

    private applyFilters(): void {
        let data = [...this.groupCodeData];

        // Show only active by default; “Show All” shows everything
        const showAll = !!this.showAllGroupCodesControl.value;
        if (!showAll) {
            // ✅ keep only active rows
            data = data.filter(x => this.asBool(x.isActive));
        }

        // Global search (simple contains across common fields)
        if (this.globalSearch) {
            const q = this.globalSearch.toLowerCase();
            data = data.filter(x =>
                [
                    x.code ?? '',
                    x.name ?? '',
                    x.groupName ?? '',
                    String(x.priority ?? ''),
                    x.value ?? ''
                ]
                    .join(' ')
                    .toLowerCase()
                    .includes(q)
            );
        }

        // Column-wise filter
        if (this.filterColumn && this.filterValue) {
            const v = this.filterValue.toLowerCase();
            data = data.filter(x => `${(x as any)[this.filterColumn] ?? ''}`.toLowerCase().includes(v));
        }

        this.filteredData = data;
        this._cdr.markForCheck();
    }
    asBool(v: any): boolean {
    return v === true || v === 1 || v === '1' || v === 'true';
    }
    rowClass = (row: any) => ({
    'text-danger': !this.asBool(row?.isActive)
    });
    // getGroupCodeData(fetchAll: boolean = false): void {
    //     this.loading = true;
    //     //const isActiveFilter = this.showAllUsersControl.value ? false : true;
    //     const isActiveFilter = !this.showAllGroupCodesControl.value;
    //     const pageSizeToUse = fetchAll ? 99999 : this.pageSize;
    //     const pageIndexToUse = fetchAll ? 1 : this.pageIndex + 1;
    //     this.groupCodeService.getGroupCodes(pageIndexToUse, pageSizeToUse, isActiveFilter).subscribe(
    //         (response) => {
    //             this.loading = false;
    //             this.groupCodeData = response.items;
    //             this.filteredData = [...this.groupCodeData];
    //             this.totalItems = response.totalCount ?? this.groupCodeData.length;
    //             this._cdr.markForCheck();
    //         },
    //         (error) => {
    //             this.loading = false;
    //             this.notificationService.error("Failed to load user data.");
    //             this._cdr.markForCheck();
    //         }
    //     );
    // }
    onSearch(searchText: string): void {
        if (!searchText) {
            this.pageIndex = 0;
            this.getGroupCodes();
        } else {
            this.getGroupCodes();
            setTimeout(() => {
                this.filteredData = this.groupCodeData.filter(groupCode =>
                    Object.values(groupCode).join(' ').toLowerCase().includes(searchText.toLowerCase())
                );
                this.totalItems = this.filteredData.length;
                this.pageIndex = 0;
                this._cdr.markForCheck();
            }, 300);
        }
    }


    onPageChange(e: PageEvent) {
        this.pageIndex = e.pageIndex;
        this.pageSize = e.pageSize;
        this.getGroupCodes();
    }

    onColumnFilter(event: { prop: string; value: string }) {
        this.pageIndex = 0;
        this.filterColumn = event?.prop || '';
        this.filterValue = (event?.value ?? '').trim();
        this.applyFilters();
    }

    toggleActivate(groupCodeId: number | string, isActive: boolean) {
        const id = Number(groupCodeId);                      // ensure number
        const verb = isActive ? 'Activate' : 'Deactivate';  // reflects new state
        if (!confirm(`Are you sure you want to ${verb} this group code?`)) {
            return;
        }

        this.groupCodeService.toggleActivate(id, isActive).subscribe({
            next: (res) => {
                if (res?.isSuccess) {
                    this.getGroupCodes();                          // refresh list
                    this.notificationService.success(`Group code ${verb.toLowerCase()}d successfully.`);
                } else {
                    this.notificationService.warning(res?.message || 'Operation failed.');
                }
            },
            error: () => this.notificationService.error('Something went wrong.')
        });
    }

    updateSearch(search: { [key: string]: any }) {
        this.searchData = Object.assign({}, search);
    }

    isActiveRow(row: any): any {
        return {
            'text-danger': !this.asBool(row.isActive)   // red only when NOT active
        };
    }

}
