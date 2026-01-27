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
        status: ['Draft', Validators.required],
        residentUserId: [''],
        flatId: [''],
        residentNameSnapshot: [''],
        contactNumberSnapshot: [''],
        isChargeableSnapshot: [null],
        amountBeforeTax: [''],
        taxAmount: [''],
        depositAmount: [''],
        discountAmount: [''],
        convenienceFee: [''],
        totalPayable: [''],
        requiresApprovalSnapshot: [null],
        approvedBy: [''],
        approvedOn: [''],
        rejectionReason: [''],
        cancelledBy: [''],
        cancelledOn: [''],
        cancellationReason: [''],
        refundStatus: ['']
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
                status: res.status,
                residentUserId: res.residentUserId,
                flatId: res.flatId,
                residentNameSnapshot: res.residentNameSnapshot,
                contactNumberSnapshot: res.contactNumberSnapshot,
                isChargeableSnapshot: res.isChargeableSnapshot,
                amountBeforeTax: res.amountBeforeTax,
                taxAmount: res.taxAmount,
                depositAmount: res.depositAmount,
                discountAmount: res.discountAmount,
                convenienceFee: res.convenienceFee,
                totalPayable: res.totalPayable,
                requiresApprovalSnapshot: res.requiresApprovalSnapshot,
                approvedBy: res.approvedBy,
                approvedOn: this.toDateInput(res.approvedOn),
                rejectionReason: res.rejectionReason,
                cancelledBy: res.cancelledBy,
                cancelledOn: this.toDateInput(res.cancelledOn),
                cancellationReason: res.cancellationReason,
                refundStatus: res.refundStatus
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
            residentUserId: this.toNumber(formValue.residentUserId),
            flatId: this.toNumber(formValue.flatId),
            amountBeforeTax: this.toNumber(formValue.amountBeforeTax),
            taxAmount: this.toNumber(formValue.taxAmount),
            depositAmount: this.toNumber(formValue.depositAmount),
            discountAmount: this.toNumber(formValue.discountAmount),
            convenienceFee: this.toNumber(formValue.convenienceFee),
            totalPayable: this.toNumber(formValue.totalPayable),
            approvedBy: this.toNumber(formValue.approvedBy),
            cancelledBy: this.toNumber(formValue.cancelledBy),
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

    private toNumber(value: string | number | null | undefined): number | null {
        if (value === null || value === undefined || value === '') {
            return null;
        }
        const parsed = Number(value);
        return Number.isNaN(parsed) ? null : parsed;
    }
}
