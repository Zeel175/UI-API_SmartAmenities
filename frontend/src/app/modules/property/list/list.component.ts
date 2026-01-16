import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { PropertyService } from '../property.service';
import { CommonModule, DatePipe } from '@angular/common';
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
import { ExcelDownloadComponent } from 'app/shared/components/excel-download/excel-download.component';
import { EmailSenderComponent } from 'app/shared/components/email-sender/email-sender.component';
import { User } from 'app/model';
import { ApplicationPage, PermissionType } from 'app/core';
import { AuthService } from 'app/core/auth/auth.service';
import { ToastrService } from 'ngx-toastr';
import { PermissionService } from 'app/core/service/permission.service';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { MatDialogModule } from '@angular/material/dialog';
import { EmailSenderService } from 'app/shared/components/email-sender/email-sender.service';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { Property } from 'app/model/property';

@Component({
  selector: 'user-list',
  providers: [PropertyService, DatePipe],
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
    EmailSenderComponent,
    MatDialogModule,
     ColumnFilterComponent
  ]
})
export class PropertyListComponent implements OnInit {
  propertyData: Property[] = [];
  filteredData: Property[] = [];
  filterColumn: string = '';
  filterValue = '';
  page = ApplicationPage.property;
  permissions = PermissionType;
  loading = false;
  isEmailFormVisible = false;
  loggedinUserData!: User;

  pageIndex = 1;
  pageSize = 10;
  totalItems = 0;

  IsAddPemission = false;
  IsEditPermission = false;
  IsDeletePermission = false;

  propertyColumns = [
    { name: 'Property Name', prop: 'propertyName', visible: true },
    { name: 'Contact No', prop: 'contactNo', visible: true },
    { name: 'Alias', prop: 'alias', visible: true },
    { name: 'Country', prop: 'country', visible: true },
    { name: 'State', prop: 'state', visible: true },
    { name: 'City', prop: 'city', visible: true },
    { name: 'Email', prop: 'email', visible: true },
    { name: 'WebSite', prop: 'website', visible: true },
    { name: 'Phone', prop: 'phone', visible: true },
    { name: 'GST No', prop: 'gstNo', visible: true },
    { name: 'Latitude', prop: 'latitude', visible: true },
    { name: 'Longitude', prop: 'longitude', visible: true },
    { name: 'Pan No', prop: 'panNo', visible: true },
    { name: 'Msme No', prop: 'msmeNo', visible: true }
  ];
  selectedProperty: Property;

  constructor(
    private propertyService: PropertyService,
    private userAuthService: AuthService,
    private notificationService: ToastrService,
    private permissionService: PermissionService,
    private router: Router,
    private route: ActivatedRoute,
    private shared: SharedStateService,
    private emailSenderService: EmailSenderService
  ) { }

  ngOnInit(): void {
    // permissions
    this.IsAddPemission = this.permissionService.hasPermission('Property (PER_PROPERTY) - Add');
    this.IsEditPermission = this.permissionService.hasPermission('Property (PER_PROPERTY) - Edit');
    this.IsDeletePermission = this.permissionService.hasPermission('Property (PER_PROPERTY) - Delete');

    // seed column‐visibility
    this.shared.setColumns(this.propertyColumns);

    // load data & user
    this.fetchLoggedinUserData();
    this.getPropertyData();
  }

  private fetchLoggedinUserData(): void {
    const authUser = this.userAuthService.getUser();
    if (authUser?.email) {
      this.loggedinUserData = authUser;
    } else {
      this.notificationService.error('Failed to fetch user data.');
    }
  }

  toggleEmailForm(): void {
    console.log('Toggling email form visibility');
    this.isEmailFormVisible = !this.isEmailFormVisible;
  }

  // configureMail(cfg: { subject: string; emailAddress: string; body: string }): void {
  //   console.log('Configure Mail config:', cfg);
  //   this.isEmailFormVisible = false;
  // }
  configureMail(cfg: { subject: string; emailAddress?: string; body: string }): void {
    // Save the template and close the form
    this.emailSenderService.saveTemplate({
      subject: cfg.subject,
      body: cfg.body,                 // keep tokens like @propertyName
      receptor: cfg.emailAddress      // optional default
    }).subscribe({
      next: () => {
        this.isEmailFormVisible = false;
        this.notificationService.success('Mail template saved.');
      },
      error: () => this.notificationService.error('Failed to save template.')
    });
  }

  private getPropertyData(): void {
    this.loading = true;
    this.propertyService.getProperty(this.pageIndex, this.pageSize).subscribe(
      (result: any) => {
        this.loading = false;
        this.propertyData = result.items;
        this.totalItems = result.totalCount;
        this.applyFilters();
      },
      () => {
        this.loading = false;
        this.notificationService.error('Failed to load property data.');
      }
    );
  }

  onPageChange(evt: PageEvent): void {
    this.pageIndex = evt.pageIndex + 1;
    this.pageSize = evt.pageSize;
    this.getPropertyData();
  }

  editProperty(id: number): void {
    this.router.navigate(['edit', id], { relativeTo: this.route });
  }

  deleteProperty(id: number): void {
    if (confirm('Delete this property?')) {
      this.propertyService.deleteProperty(id).subscribe(
        () => {
          this.notificationService.success('Deleted successfully.');
          this.getPropertyData();
        },
        () => this.notificationService.error('Delete failed.')
      );
    }
  }
  sendMail(emailConfig: { receptor: string; subject: string; body: string }): void {
    this.loading = true;
    // Call email service (implement in your service)
    this.emailSenderService.sendEmail(emailConfig).subscribe(
      () => {
        console.log('Sending email:', emailConfig);
        this.notificationService.success('Email sent successfully.');
      },
      (error) => {
        console.error('Error sending email:', error);
        this.notificationService.error('Failed to send email.');
      }
    ).add(() => {
      this.loading = false; // Reset loading state
    });
  }
  onEmail(propertyId: number) {
    const row = this.propertyData.find(c => c.id === propertyId);
    if (!row) { return; }

    this.emailSenderService.getTemplate().subscribe(tpl => {
      if (!tpl) {
        this.notificationService.info('Please configure the mail template first.');
        this.isEmailFormVisible = true;
        return;
      }

      // Replace @tokens using row values; convert \n to <br> if HTML
      const subject = this.interpolate(tpl.subject, row);
      const body = this.interpolate(tpl.body, row).replace(/\n/g, '<br>');

      // Recipient: row.email -> template default -> logged-in user
      const receptor = (row as any).email || tpl.receptor || this.loggedinUserData?.email;
      if (!receptor) {
        this.notificationService.error('No recipient email available.');
        return;
      }

      this.sendMail({ receptor, subject, body });
    });
  }
  // Robust token replace: @fieldName → row[fieldName]
  private interpolate(template: string, data: any): string {
    if (!template) { return ''; }
    return template.replace(/@(\w+)/g, (_m, key) => {
      const val = data?.[key];
      return val != null ? String(val) : `@${key}`;  // leave token if not found
    });
  }
  onColumnFilter(event: { prop: string; value: string }) {
    this.filterColumn = event?.prop || '';
    this.filterValue = (event?.value ?? '').trim();
    this.applyFilters();
  }

  private applyFilters(): void {
    let data = [...this.propertyData];
    if (this.filterColumn && this.filterValue) {
      const v = this.filterValue.toLowerCase();
      data = data.filter(x => `${(x as any)[this.filterColumn] ?? ''}`.toLowerCase().includes(v));
    }
    this.filteredData = data;
  }
}
