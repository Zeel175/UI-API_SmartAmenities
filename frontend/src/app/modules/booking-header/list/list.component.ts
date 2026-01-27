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
import { BookingHeader } from 'app/model';
import { ColumnFilterComponent } from 'app/shared/components/column-filter/column-filter.component';
import { FuseListComponent } from 'app/shared/components/fuse-list/fuse-list.component';
import { ToastrService } from 'ngx-toastr';
import { BookingHeaderService } from '../booking-header.service';

@Component({
    selector: 'booking-header-list',
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
export class BookingHeaderListComponent implements OnInit {
    bookingData: BookingHeader[] = [];
    filteredData: BookingHeader[] = [];
    page = ApplicationPage.bookingHeader;
    permissions = PermissionType;
    loading = false;
    pageIndex = 1;
    pageSize = 10;
    totalItems = 0;

    IsAddPermission = false;
    IsEditPermission = false;
    IsDeletePermission = false;

    bookingColumns = [
        { name: 'Booking No', prop: 'bookingNo', visible: true },
        { name: 'Amenity', prop: 'amenityName', visible: true },
        { name: 'Society', prop: 'societyName', visible: true },
        { name: 'Resident', prop: 'residentNameSnapshot', visible: true },
        { name: 'Contact', prop: 'contactNumberSnapshot', visible: false },
        { name: 'Booking Date', prop: 'bookingDate', visible: true },
        { name: 'Total Payable', prop: 'totalPayable', visible: false },
        { name: 'Status', prop: 'status', visible: true },
        { name: 'Remarks', prop: 'remarks', visible: false }
    ];

    constructor(
        private bookingService: BookingHeaderService,
        private notificationService: ToastrService,
        private permissionService: PermissionService,
        private router: Router,
        private route: ActivatedRoute
    ) { }

    ngOnInit(): void {
        this.IsAddPermission = this.permissionService.hasPermission('Booking Header (PER_BOOKING_HEADER) - Add');
        this.IsEditPermission = this.permissionService.hasPermission('Booking Header (PER_BOOKING_HEADER) - Edit');
        this.IsDeletePermission = this.permissionService.hasPermission('Booking Header (PER_BOOKING_HEADER) - Delete');
        this.getBookingData();
    }

    private getBookingData(): void {
        this.loading = true;
        this.bookingService.getBookings(this.pageIndex, this.pageSize).subscribe(
            (result: any) => {
                this.loading = false;
                this.bookingData = result.items || result;
                this.totalItems = result.totalCount || this.bookingData.length;
                this.filteredData = this.bookingData;
            },
            () => {
                this.loading = false;
                this.notificationService.error('Failed to load booking data.');
            }
        );
    }

    onPageChange(evt: PageEvent): void {
        this.pageIndex = evt.pageIndex + 1;
        this.pageSize = evt.pageSize;
        this.getBookingData();
    }

    editBooking(id: number): void {
        this.router.navigate(['edit', id], { relativeTo: this.route });
    }

    deleteBooking(id: number): void {
        if (confirm('Delete this booking?')) {
            this.bookingService.deleteBooking(id).subscribe(
                () => {
                    this.notificationService.success('Deleted successfully.');
                    this.getBookingData();
                },
                () => this.notificationService.error('Delete failed.')
            );
        }
    }

    onColumnFilter(event: { prop: string; value: string }): void {
        if (!event || !event.prop || !event.value) {
            this.filteredData = this.bookingData;
            return;
        }

        const prop = event.prop;
        const val = (event.value || '').toString().toLowerCase();

        this.filteredData = (this.bookingData || []).filter((row: any) => {
            const cell = (row && row[prop] != null) ? row[prop] : '';
            return cell.toString().toLowerCase().includes(val);
        });
    }
}
