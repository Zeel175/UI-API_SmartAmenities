import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
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
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { BookingHeaderService } from '../booking-header.service';
import { BookingUnit } from 'app/model';
import { UnitService } from 'app/modules/unit/unit.service';
import { ResidentMasterService } from 'app/modules/resident-master/resident-master.service';

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
        MatDatepickerModule,
        MatNativeDateModule,
        CommonModule
    ]
})
export class BookingHeaderAddEditComponent implements OnInit {
    bookingId: number;
    isEditMode = false;
    amenities: any[] = [];
    amenityUnits: any[] = [];
    societies: any[] = [];
    units: any[] = [];
    residentUsers: any[] = [];
    hasAmenityUnits = false;
    isLoadingBooking = false;

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
        bookingUnits: this.fb.array([]),
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
        private unitService: UnitService,
        private residentMasterService: ResidentMasterService,
        private route: ActivatedRoute,
        private router: Router,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Booking Header (PER_BOOKING_HEADER) - View');
        this.loadAmenities();
        this.loadSocieties();
        this.loadUnits();
        this.ensureBookingUnitRow();
        this.frmBooking.get('amenityId')?.valueChanges.subscribe((amenityId) => {
            if (!this.isLoadingBooking) {
                this.applyAmenitySnapshots(amenityId ? +amenityId : null);
            }
            this.resetBookingUnits();
            if (amenityId) {
                this.loadAmenityUnits(+amenityId);
            } else {
                this.setAmenityUnits([]);
            }
        });
        this.frmBooking.get('flatId')?.valueChanges.subscribe((unitId) => {
            if (this.isLoadingBooking) {
                return;
            }
            this.handleUnitSelection(unitId ? +unitId : null);
        });
        this.frmBooking.get('residentUserId')?.valueChanges.subscribe((residentUserId) => {
            if (this.isLoadingBooking) {
                return;
            }
            this.applyResidentSnapshot(residentUserId ? +residentUserId : null);
        });
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

    private loadUnits(): void {
        this.unitService.getAllUnits().subscribe((res: any) => {
            this.units = res.items || res;
        });
    }

    private handleUnitSelection(unitId: number | null): void {
        this.residentUsers = [];
        this.frmBooking.patchValue({
            residentUserId: '',
            residentNameSnapshot: '',
            contactNumberSnapshot: ''
        }, { emitEvent: false });

        if (!unitId) {
            return;
        }

        this.residentMasterService.getUsersByUnit(unitId).subscribe((users: any) => {
            this.residentUsers = users?.items || users || [];
        });
    }

    private loadAmenityUnits(amenityId: number, bookingUnits: BookingUnit[] = []): void {
        this.bookingService.getAmenityUnitsByAmenityId(amenityId).subscribe((res: any) => {
            this.setAmenityUnits(res.items || res);
            if (bookingUnits.length) {
                this.setBookingUnits(bookingUnits);
            } else {
                this.ensureBookingUnitRow();
            }
        }, () => {
            this.setAmenityUnits([]);
            this.ensureBookingUnitRow();
        });
    }

    get bookingUnitsFormArray(): FormArray {
        return this.frmBooking.get('bookingUnits') as FormArray;
    }

    addBookingUnitRow(unit?: BookingUnit): void {
        this.bookingUnitsFormArray.push(this.fb.group({
            amenityUnitId: [unit?.amenityUnitId ?? '', this.hasAmenityUnits ? Validators.required : []],
            capacityReserved: [unit?.capacityReserved ?? null, [Validators.min(1)]]
        }));
    }

    removeBookingUnitRow(index: number): void {
        this.bookingUnitsFormArray.removeAt(index);
        if (!this.bookingUnitsFormArray.length) {
            this.addBookingUnitRow();
        }
    }

    private resetBookingUnits(): void {
        this.bookingUnitsFormArray.clear();
    }

    private ensureBookingUnitRow(): void {
        if (this.hasAmenityUnits && !this.bookingUnitsFormArray.length) {
            this.addBookingUnitRow();
        }
    }

    private setBookingUnits(units: BookingUnit[]): void {
        this.resetBookingUnits();
        units.forEach(unit => this.addBookingUnitRow(unit));
    }

    private getBookingDetails(): void {
        this.isLoadingBooking = true;
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

            const units = res.bookingUnits || [];
            if (res.flatId) {
                this.loadResidentsByUnit(res.flatId, res.residentUserId);
            }
            if (res.amenityId) {
                this.loadAmenityUnits(res.amenityId, units);
            } else if (units.length) {
                this.setBookingUnits(units);
            } else {
                this.ensureBookingUnitRow();
            }
            this.isLoadingBooking = false;
        }, () => {
            this.isLoadingBooking = false;
        });
    }

    save(): void {
        const formValue = this.frmBooking.getRawValue();
        const bookingUnits = (formValue.bookingUnits || [])
            .filter((unit: any) => this.toNumber(unit.amenityUnitId))
            .map((unit: any) => {
            const selectedUnit = this.amenityUnits.find(item => item.id === +unit.amenityUnitId);
            return {
                id: unit.id,
                amenityUnitId: this.toNumber(unit.amenityUnitId),
                capacityReserved: this.toNumber(unit.capacityReserved),
                unitNameSnapshot: selectedUnit?.unitName ?? selectedUnit?.name ?? null
            };
        });
        const payload = {
            ...formValue,
            amenityId: +formValue.amenityId,
            societyId: +formValue.societyId,
            bookingDate: this.toDateValue(formValue.bookingDate),
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
            approvedOn: this.toDateValue(formValue.approvedOn),
            cancelledOn: this.toDateValue(formValue.cancelledOn),
            bookingUnits,
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

    private setAmenityUnits(units: any[]): void {
        this.amenityUnits = units || [];
        this.hasAmenityUnits = this.amenityUnits.length > 0;
        if (!this.hasAmenityUnits) {
            this.resetBookingUnits();
        }
        this.updateAmenityUnitValidators();
    }

    private updateAmenityUnitValidators(): void {
        this.bookingUnitsFormArray.controls.forEach(control => {
            const amenityUnitControl = control.get('amenityUnitId');
            if (!amenityUnitControl) {
                return;
            }
            amenityUnitControl.setValidators(this.hasAmenityUnits ? Validators.required : null);
            amenityUnitControl.updateValueAndValidity({ emitEvent: false });
        });
    }

    private applyAmenitySnapshots(amenityId: number | null): void {
        if (!amenityId) {
            this.frmBooking.patchValue({
                isChargeableSnapshot: null,
                requiresApprovalSnapshot: null,
                amountBeforeTax: '',
                taxAmount: '',
                depositAmount: '',
                totalPayable: ''
            }, { emitEvent: false });
            return;
        }

        const amenity = this.amenities.find(item => item.id === amenityId);
        if (!amenity) {
            return;
        }

        const baseRate = this.toNumber(amenity.baseRate);
        const depositAmount = this.toNumber(amenity.securityDeposit);
        const taxPercentage = this.toNumber(amenity.taxPercentage) ?? 0;
        const normalizedBaseRate = baseRate ?? 0;
        const normalizedDeposit = depositAmount ?? 0;
        const taxAmount = amenity.taxApplicable && baseRate !== null
            ? (normalizedBaseRate * taxPercentage) / 100
            : 0;
        const totalPayable = normalizedBaseRate + taxAmount + normalizedDeposit;

        this.frmBooking.patchValue({
            isChargeableSnapshot: amenity.isChargeable ?? null,
            requiresApprovalSnapshot: amenity.requiresApproval ?? null,
            amountBeforeTax: baseRate ?? '',
            taxAmount: baseRate === null ? '' : taxAmount,
            depositAmount: depositAmount ?? '',
            totalPayable: baseRate === null && depositAmount === null ? '' : totalPayable
        }, { emitEvent: false });
    }

    private loadResidentsByUnit(unitId: number, selectedUserId?: number): void {
        this.residentMasterService.getUsersByUnit(unitId).subscribe((users: any) => {
            this.residentUsers = users?.items || users || [];
            if (selectedUserId) {
                this.applyResidentSnapshot(selectedUserId, true);
            }
        });
    }

    private applyResidentSnapshot(residentUserId: number | null, preserveExisting = false): void {
        if (!residentUserId) {
            return;
        }

        const selectedUser = this.residentUsers.find((user: any) => user.id === residentUserId);
        if (!selectedUser) {
            return;
        }

        const displayName = this.getResidentDisplayName(selectedUser);
        const contactNumber = selectedUser.mobile || selectedUser.contactNumber || selectedUser.phoneNumber || '';

        this.frmBooking.patchValue({
            residentNameSnapshot: preserveExisting
                ? this.frmBooking.get('residentNameSnapshot')?.value || displayName
                : displayName,
            contactNumberSnapshot: preserveExisting
                ? this.frmBooking.get('contactNumberSnapshot')?.value || contactNumber
                : contactNumber
        }, { emitEvent: false });
    }

    getResidentDisplayName(user: any): string {
        if (!user) {
            return '';
        }
        if (user.fullName) {
            return user.fullName;
        }
        const firstName = user.firstName || '';
        const lastName = user.lastName || '';
        const combined = `${firstName} ${lastName}`.trim();
        return combined || user.userName || user.name || '';
    }

    private toDateInput(value: string | null | undefined): Date | null {
        if (!value) {
            return null;
        }
        const date = new Date(value);
        return Number.isNaN(date.getTime()) ? null : date;
    }

    private toDateValue(value: Date | string | null | undefined): string | null {
        if (!value) {
            return null;
        }
        if (value instanceof Date) {
            if (Number.isNaN(value.getTime())) {
                return null;
            }
            const year = value.getFullYear();
            const month = (value.getMonth() + 1).toString().padStart(2, '0');
            const day = value.getDate().toString().padStart(2, '0');
            return `${year}-${month}-${day}`;
        }
        return value;
    }

    private toNumber(value: string | number | null | undefined): number | null {
        if (value === null || value === undefined || value === '') {
            return null;
        }
        const parsed = Number(value);
        return Number.isNaN(parsed) ? null : parsed;
    }
}
