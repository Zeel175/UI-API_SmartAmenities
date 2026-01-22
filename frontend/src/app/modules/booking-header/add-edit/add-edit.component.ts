import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { BookingHeaderService } from '../booking-header.service';

@Component({
    selector: 'booking-header-add-edit',
    standalone: true,
    templateUrl: './add-edit.component.html',
    styleUrls: ['./add-edit.component.scss'],
    imports: [
        ReactiveFormsModule,
        FormsModule,
        RouterModule,
        MatFormFieldModule,
        MatCardModule,
        MatInputModule,
        MatSelectModule,
        MatIconModule,
        MatButtonModule,
        CommonModule
    ]
})
export class BookingHeaderAddEditComponent implements OnInit {
    bookingId: number;
    isEditMode = false;
    amenities: any[] = [];
    societies: any[] = [];

    statusOptions = [
        'Draft',
        'PendingApproval',
        'Approved',
        'Rejected',
        'Confirmed',
        'Cancelled',
        'Completed',
        'NoShow'
    ];

    frmBooking = this.fb.group({
        amenityId: ['', Validators.required],
        societyId: ['', Validators.required],
        bookingNo: [''],
        bookingDate: ['', Validators.required],
        remarks: [''],
        status: ['Draft', Validators.required]
    });

    page = ApplicationPage.bookingHeader;
    permissions = PermissionType;
    IsViewPermission = false;

    constructor(
        private fb: FormBuilder,
        private bookingService: BookingHeaderService,
        private route: ActivatedRoute,
        private router: Router,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Booking Header (PER_BOOKING_HEADER) - View');
        this.loadAmenities();
        this.loadSocieties();
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.bookingId = +params['id'];
                this.getBookingDetails();
            }
        });
    }

    private loadAmenities(): void {
        this.bookingService.getAmenities().subscribe((res: any) => {
            this.amenities = res.items || res;
        });
    }

    private loadSocieties(): void {
        this.bookingService.getSocieties().subscribe((res: any) => {
            this.societies = res.items || res;
        });
    }

    private getBookingDetails(): void {
        this.bookingService.getBookingById(this.bookingId).subscribe((res: any) => {
            this.frmBooking.patchValue({
                amenityId: res.amenityId,
                societyId: res.societyId,
                bookingNo: res.bookingNo,
                bookingDate: this.toDateInput(res.bookingDate),
                remarks: res.remarks,
                status: res.status
            });
        });
    }

    save(): void {
        const formValue = this.frmBooking.getRawValue();
        const payload = {
            ...formValue,
            amenityId: +formValue.amenityId,
            societyId: +formValue.societyId,
            bookingDate: formValue.bookingDate,
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.bookingId : 0
        };

        const request$ = this.isEditMode
            ? this.bookingService.updateBooking(payload)
            : this.bookingService.addBooking(payload);

        request$.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/booking-header']);
        }, () => this.notificationService.error('Save failed.'));
    }

    cancel(): void {
        this.router.navigate(['/booking-header']);
    }

    private toDateInput(value: string | null | undefined): string {
        if (!value) {
            return '';
        }
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return '';
        }
        const year = date.getFullYear();
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        const day = date.getDate().toString().padStart(2, '0');
        return `${year}-${month}-${day}`;
    }
}
