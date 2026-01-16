import {
    AfterViewInit,
    ChangeDetectionStrategy,
    ChangeDetectorRef,
    Component,
    OnDestroy,
    OnInit,
    TemplateRef,
    ViewChild,
    ViewEncapsulation,
} from '@angular/core';
import { MatPaginator, MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { FormsModule, ReactiveFormsModule, UntypedFormControl } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { fuseAnimations } from '@fuse/animations';
import { UserService } from '../user.service';
import { ActivatedRoute, Router, RouterModule } from '@angular/router'; // Import RouterModule
import { ToastrService } from 'ngx-toastr';
import { User } from 'app/model';
import { PermissionService } from 'app/core/service/permission.service';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { debounceTime } from 'rxjs';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';import { ApplicationPage, PermissionType } from 'app/core';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { ExcelDownloadComponent } from 'app/shared/components/excel-download/excel-download.component';
import { SharedStateService } from 'app/shared/services/shared-state.service';

@Component({
    selector: 'user-list',
    providers: [UserService, DatePipe],
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
        FuseListComponent,
        ExcelDownloadComponent,
        ColumnFilterComponent
       // ColumnFiltersComponent
    ]
})
export class UserListComponent implements OnInit,OnDestroy{
    userData: User[] = [];
    filteredData: User[] = [];
    filterColumn: string = '';
    filterValue = '';
  page = ApplicationPage.user;
  permissions = PermissionType;
  loading = false;

  pageIndex = 1;
  pageSize = 10;
  totalItems = 0;

  IsAddPemission = false;
  IsEditPermission = false;
  IsDeletePermission = false;

  @ViewChild('createdDateTemplate', { static: true })
  createdDateTemplate!: TemplateRef<any>;

  private readonly baseUserColumns = Object.freeze([
  { name: 'User Name',   prop: 'userName',    visible: true },
  { name: 'Email',       prop: 'email',       visible: true },
  { name: 'Phone Number',prop: 'phoneNumber', visible: true },
  { name: 'Created Date',prop: 'createdDate', visible: true } // template added in reset()
]);

userColumns: { name: string; prop: string; visible: boolean; cellTemplate?: TemplateRef<any> }[] = [];


  constructor(
    private userService: UserService,
    private notificationService: ToastrService,
    private permissionService: PermissionService,
    private router: Router,
    private route: ActivatedRoute,
    private shared: SharedStateService 
  ) {}

  ngOnDestroy(): void {
    this.userColumns = [];
  }

private resetColumns(): void {
  this.userColumns = this.baseUserColumns.map(c =>
    c.prop === 'createdDate' ? { ...c, cellTemplate: this.createdDateTemplate } : { ...c }
  );
}
  ngOnInit(): void {
    this.IsAddPemission = this.permissionService.hasPermission('User (PER_USER) - Add');
    this.IsEditPermission = this.permissionService.hasPermission('User (PER_USER) - Edit');
    this.IsDeletePermission = this.permissionService.hasPermission('User (PER_USER) - Delete');

    this.resetColumns();
    
    this.getUserData();
  }

  private getUserData() {
    this.loading = true;
    this.userService.getUsers(this.pageIndex, this.pageSize).subscribe(
      (result: any) => {
        this.loading = false;
        this.userData = result.items;
        console.log(this.userData);
        this.totalItems = result.totalCount;
        this.applyFilters();
      },
      (error) => {
        this.loading = false;
        console.error(error);
      }
    );
  }

  onPageChange(event: PageEvent): void {
    // Fuse-list gives back zero-based pageIndex
    this.pageIndex = event.pageIndex + 1;
    this.pageSize = event.pageSize;
    this.getUserData();
  }

  editUser(id: number): void {
  this.router.navigate(['edit', id], { relativeTo: this.route });
}

  deleteUser(id: any): void {
    if (confirm('Are you sure you want to delete User?')) {
      this.userService.deleteUser(id).subscribe(
        () => this.getUserData(),
        () => this.notificationService.error('Something went wrong.')
      );
    }
  }
onColumnFilter(event: { prop: string; value: string }) {
    this.filterColumn = event?.prop || '';
    this.filterValue = (event?.value ?? '').trim();
    this.pageIndex = 1; 
    this.applyFilters();
  }

  private applyFilters(): void {
    let data = [...this.userData];
    if (this.filterColumn && this.filterValue) {
      const v = this.filterValue.toLowerCase();
      data = data.filter(x => `${(x as any)[this.filterColumn] ?? ''}`.toLowerCase().includes(v));
    }
    this.filteredData = data;
  }
  // addUser(): void {
  //   this.router.navigate(['..', 'add'], { relativeTo: this.route });
  // }
}