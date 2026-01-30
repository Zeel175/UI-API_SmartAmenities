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
import { BookingSlotAvailability, BookingUnit } from 'app/model';
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
    bookingSlotRequired = false;
    isLoadingBooking = false;
    availableSlotsByRow: Record<number, BookingSlotAvailability[]> = {};

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
        bookingDate: this.fb.control<Date | null>(null, Validators.required),
        remarks: [''],
        status: ['Draft', Validators.required],
        bookingUnits: this.fb.array([]),
        residentUserId: [''],
        flatId: [''],
        residentNameSnapshot: [''],
        contactNumberSnapshot: [''],
        isChargeableSnapshot: [null],
        amountBeforeTax: this.fb.control<number | string | null>(''),
        taxAmount: this.fb.control<number | string | null>(''),
        depositAmount: this.fb.control<number | string | null>(''),
        discountAmount: [''],
        convenienceFee: [''],
        totalPayable: this.fb.control<number | string | null>(''),
        requiresApprovalSnapshot: [null],
        approvedBy: [''],
        approvedOn: this.fb.control<Date | null>(null),
        rejectionReason: [''],
        cancelledBy: [''],
        cancelledOn: this.fb.control<Date | null>(null),
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
        this.bookingUnitsFormArray.valueChanges.subscribe(() => {
            if (this.isLoadingBooking) {
                return;
            }
            this.applyChargesFromAmenityUnits();
        });
        this.frmBooking.get('bookingDate')?.valueChanges.subscribe(() => {
            if (this.isLoadingBooking) {
                return;
            }
            this.reloadSlotsForAllRows();
        });
        this.frmBooking.get('amenityId')?.valueChanges.subscribe((amenityId) => {
            if (!this.isLoadingBooking) {
                this.applyAmenitySnapshots(amenityId ? +amenityId : null);
            }
            this.resetBookingUnits();
            this.availableSlotsByRow = {};
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
            const amenityId = this.toNumber(this.frmBooking.get('amenityId')?.value);
            if (amenityId) {
                this.updateSlotRequirement(amenityId);
                if (this.isEditMode && this.bookingSlotRequired) {
                    this.reloadSlotsForAllRows();
                }
            }
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
            this.applyChargesFromAmenityUnits();
            if (this.bookingSlotRequired) {
                this.reloadSlotsForAllRows();
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
        const group = this.fb.group({
            amenityUnitId: [unit?.amenityUnitId ?? '', this.hasAmenityUnits ? Validators.required : []],
            capacityReserved: [unit?.capacityReserved ?? null, [Validators.min(1)]],
            slotSelection: [null],
            slotStartDateTime: [unit?.bookingSlot?.slotStartDateTime ?? null],
            slotEndDateTime: [unit?.bookingSlot?.slotEndDateTime ?? null],
            bookingSlotId: [unit?.bookingSlot?.id ?? null],
            slotStatus: [unit?.bookingSlot?.slotStatus ?? 'Reserved']
        });
        this.bookingUnitsFormArray.push(group);
        this.registerBookingUnitListeners(group);
    }

    removeBookingUnitRow(index: number): void {
        this.bookingUnitsFormArray.removeAt(index);
        this.availableSlotsByRow = {};
        if (!this.bookingUnitsFormArray.length) {
            this.addBookingUnitRow();
        } else {
            this.reloadSlotsForAllRows();
        }
    }

    private resetBookingUnits(): void {
        this.bookingUnitsFormArray.clear();
        this.availableSlotsByRow = {};
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
            this.updateSlotRequirement(res.amenityId ?? null);
            this.reloadSlotsForAllRows();
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
            const slotSelection = unit.slotSelection as BookingSlotAvailability | null;
            return {
                id: unit.id,
                amenityUnitId: this.toNumber(unit.amenityUnitId),
                capacityReserved: this.toNumber(unit.capacityReserved),
                unitNameSnapshot: selectedUnit?.unitName ?? selectedUnit?.name ?? null,
                bookingSlot: slotSelection
                    ? {
                         id: this.toNumber(unit.bookingSlotId),
                        slotStartDateTime: slotSelection.slotStartDateTime,
                        slotEndDateTime: slotSelection.slotEndDateTime,
                        slotStatus: unit.slotStatus ?? 'Reserved'
                    }
                    : null
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

    private registerBookingUnitListeners(group: any): void {
        group.get('amenityUnitId')?.valueChanges.subscribe(() => {
            if (this.isLoadingBooking) {
                return;
            }
            const index = this.bookingUnitsFormArray.controls.indexOf(group);
            if (index >= 0) {
                this.clearSlotSelection(index);
                this.loadSlotsForRow(index);
            }
        });
        group.get('slotSelection')?.valueChanges.subscribe(() => {
            if (this.isLoadingBooking) {
                return;
            }
            const index = this.bookingUnitsFormArray.controls.indexOf(group);
            if (index >= 0) {
                this.applySlotSelection(index);
            }
        });
    }

    private reloadSlotsForAllRows(): void {
        this.bookingUnitsFormArray.controls.forEach((control, index) => {
            this.loadSlotsForRow(index);
        });
    }

    private loadSlotsForRow(index: number): void {
        if (!this.bookingSlotRequired) {
            this.clearSlotSelection(index);
            return;
        }
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return;
        }
        const amenityId = this.toNumber(this.frmBooking.get('amenityId')?.value);
        const amenityUnitId = this.toNumber(group.get('amenityUnitId')?.value);
        const bookingDate = this.toDateValue(this.frmBooking.get('bookingDate')?.value);
        if (!amenityId || !amenityUnitId || !bookingDate) {
            this.clearSlotSelection(index);
            return;
        }
        this.bookingService.getAvailableSlots(amenityId, amenityUnitId, bookingDate, this.isEditMode ? this.bookingId : null)
            .subscribe((slots) => {
                const slotOptions = slots || [];
                if (!slotOptions.length) {
                    this.clearSlotSelection(index);
                    return;
                }
                this.availableSlotsByRow[index] = this.ensureSelectedSlotOption(index, slotOptions);
                this.syncSlotSelection(index);
            }, () => {
                this.clearSlotSelection(index);
            });
    }

    private syncSlotSelection(index: number): void {
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return;
        }
        const slotStartDateTime = group.get('slotStartDateTime')?.value;
        const slotEndDateTime = group.get('slotEndDateTime')?.value;
        if (!slotStartDateTime || !slotEndDateTime) {
            return;
        }
        const availableSlots = this.availableSlotsByRow[index] || [];
        const match = availableSlots.find(slot =>
            slot.slotStartDateTime === slotStartDateTime && slot.slotEndDateTime === slotEndDateTime);
        if (match) {
            group.patchValue({ slotSelection: match, bookingSlotId: match.id ?? null }, { emitEvent: false });
            return;
        }
        const fallbackSlot = this.buildSlotFallback(slotStartDateTime, slotEndDateTime);
        group.patchValue({ slotSelection: fallbackSlot }, { emitEvent: false });
    }

    applySlotSelection(index: number): void {
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return;
        }
        const slot = group.get('slotSelection')?.value as BookingSlotAvailability | null;
        if (!slot) {
            group.patchValue({
                bookingSlotId: null,
                slotStartDateTime: null,
                slotEndDateTime: null
            }, { emitEvent: false });
            return;
        }
        group.patchValue({
            bookingSlotId: slot.id ?? null,
            slotStartDateTime: slot.slotStartDateTime,
            slotEndDateTime: slot.slotEndDateTime
        }, { emitEvent: false });
        this.applyChargesFromAmenityUnits();
    }

    getSlotOptions(index: number): BookingSlotAvailability[] {
        const available = this.availableSlotsByRow[index] || [];
        const selectedSlot = this.getSelectedSlot(index);

        return available.filter(slot => {
            if (selectedSlot && this.getSlotKey(slot) === this.getSlotKey(selectedSlot)) {
                return true;
            }
            const remainingCapacity = this.getRemainingSlotCapacity(slot, index);
            if (remainingCapacity === null) {
                return true;
            }
            return remainingCapacity > 0;
        });
    }

    hasSlotOptions(index: number): boolean {
        if (!this.bookingSlotRequired) {
            return false;
        }
        return this.getSlotOptions(index).length > 0;
    }

    getSlotLabel(slot: BookingSlotAvailability, index?: number): string {
        const start = this.formatSlotTime(slot.slotStartDateTime);
        const end = this.formatSlotTime(slot.slotEndDateTime);
        if (slot.capacityPerSlot === undefined || slot.availableCapacity === undefined) {
            return `${start} - ${end}`;
        }
        const remainingCapacity = index === undefined
            ? slot.availableCapacity
            : this.getRemainingSlotCapacity(slot, index) ?? slot.availableCapacity;
        return `${start} - ${end} (Cap: ${remainingCapacity}/${slot.capacityPerSlot})`;
    }

    private formatSlotTime(value: string): string {
        const date = new Date(value);
        if (Number.isNaN(date.getTime())) {
            return value;
        }
        return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }

    private getSlotKey(slot: BookingSlotAvailability): string {
        return `${slot.slotStartDateTime}|${slot.slotEndDateTime}`;
    }

    private getSelectedSlot(index: number): BookingSlotAvailability | null {
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return null;
        }
        return group.get('slotSelection')?.value as BookingSlotAvailability | null;
    }

    private getRemainingSlotCapacity(slot: BookingSlotAvailability, index: number): number | null {
        if (slot.availableCapacity === undefined) {
            return null;
        }
        const targetSlotKey = this.getSlotKey(slot);
        const targetUnitId = this.toNumber(this.bookingUnitsFormArray.at(index)?.get('amenityUnitId')?.value);
        const reservedCapacity = this.bookingUnitsFormArray.controls.reduce((total, control, idx) => {
            if (idx === index) {
                return total;
            }
            const unitId = this.toNumber(control.get('amenityUnitId')?.value);
            if (!unitId || unitId !== targetUnitId) {
                return total;
            }
            const selectedSlot = control.get('slotSelection')?.value as BookingSlotAvailability | null;
            if (!selectedSlot || this.getSlotKey(selectedSlot) !== targetSlotKey) {
                return total;
            }
            const reserved = this.toNumber(control.get('capacityReserved')?.value);
            return total + (reserved ?? 0);
        }, 0);
        return Math.max(slot.availableCapacity - reservedCapacity, 0);
    }

    private buildSlotFallback(slotStartDateTime: string, slotEndDateTime: string): BookingSlotAvailability {
        return {
            slotStartDateTime,
            slotEndDateTime,
            capacityPerSlot: 0,
            availableCapacity: 0,
            isChargeable: false,
            refundableDeposit: false,
            taxApplicable: false
        };
    }

    private ensureSelectedSlotOption(index: number, slots: BookingSlotAvailability[]): BookingSlotAvailability[] {
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return slots;
        }
        const slotStartDateTime = group.get('slotStartDateTime')?.value;
        const slotEndDateTime = group.get('slotEndDateTime')?.value;
        if (!slotStartDateTime || !slotEndDateTime) {
            return slots;
        }
        const slotKey = `${slotStartDateTime}|${slotEndDateTime}`;
        if (slots.some(slot => this.getSlotKey(slot) === slotKey)) {
            return slots;
        }
        return [this.buildSlotFallback(slotStartDateTime, slotEndDateTime), ...slots];
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
            this.updateSlotRequirement(null);
            this.frmBooking.patchValue({
                requiresApprovalSnapshot: null
            }, { emitEvent: false });
            this.resetChargesSnapshot();
            return;
        }

        const amenity = this.amenities.find(item => item.id === amenityId);
        if (!amenity) {
            return;
        }

        this.updateSlotRequirement(amenityId);
        this.frmBooking.patchValue({
            requiresApprovalSnapshot: amenity.requiresApproval ?? null
        }, { emitEvent: false });
    }

    private updateSlotRequirement(amenityId: number | null): void {
        if (!amenityId) {
            this.bookingSlotRequired = false;
            this.availableSlotsByRow = {};
            this.clearAllSlotSelections();
            return;
        }

        const amenity = this.amenities.find(item => item.id === amenityId);
        this.bookingSlotRequired = !!amenity?.bookingSlotRequired;
        if (!this.bookingSlotRequired) {
            this.availableSlotsByRow = {};
            this.clearAllSlotSelections();
        }
    }

    private clearAllSlotSelections(): void {
        this.bookingUnitsFormArray.controls.forEach((_, index) => {
            this.clearSlotSelection(index);
        });
    }

    private clearSlotSelection(index: number): void {
        const group = this.bookingUnitsFormArray.at(index);
        if (!group) {
            return;
        }
        this.availableSlotsByRow[index] = [];
        group.patchValue({
            slotSelection: null,
            bookingSlotId: null,
            slotStartDateTime: null,
            slotEndDateTime: null
        }, { emitEvent: false });
    }

    private applyChargesFromAmenityUnits(): void {
        const selectedUnitIds = this.bookingUnitsFormArray.controls
            .map(control => this.toNumber(control.get('amenityUnitId')?.value))
            .filter((id): id is number => typeof id === 'number');

        if (!selectedUnitIds.length) {
            this.resetChargesSnapshot();
            return;
        }

        let baseRateTotal = 0;
        let depositTotal = 0;
        let taxTotal = 0;
        let hasChargeable = false;
        let hasNonChargeable = false;
        let hasAnyRate = false;
        let hasAnyDeposit = false;
        let hasAnyTax = false;

        this.bookingUnitsFormArray.controls.forEach((control) => {
            const unitId = this.toNumber(control.get('amenityUnitId')?.value);
            if (!unitId) {
                return;
            }
            const unit = this.amenityUnits.find(item => item.id === unitId);
            const slotSelection = control.get('slotSelection')?.value as BookingSlotAvailability | null;
            const slotRate = slotSelection ? this.toNumber(slotSelection.baseRate ?? slotSelection.slotCharge) : null;
            const baseRate = slotRate ?? this.toNumber(unit?.baseRate);
            const depositAmount = slotSelection
                ? this.toNumber(slotSelection.securityDeposit)
                : this.toNumber(unit?.securityDeposit);
            const taxPercentage = slotSelection
                ? this.toNumber(slotSelection.taxPercentage) ?? 0
                : this.toNumber(unit?.taxPercentage) ?? 0;
            const taxApplicable = slotSelection ? slotSelection.taxApplicable : unit?.taxApplicable;

            if (baseRate !== null) {
                baseRateTotal += baseRate;
                hasAnyRate = true;
            }

            if (depositAmount !== null) {
                depositTotal += depositAmount;
                hasAnyDeposit = true;
            }

            if (taxApplicable && baseRate !== null) {
                taxTotal += (baseRate * taxPercentage) / 100;
                hasAnyTax = true;
            }

            if ((slotSelection ? slotSelection.isChargeable : unit?.isChargeable) === true) {
                hasChargeable = true;
            } else if ((slotSelection ? slotSelection.isChargeable : unit?.isChargeable) === false) {
                hasNonChargeable = true;
            }
        });

        const isChargeableSnapshot = hasChargeable ? true : hasNonChargeable ? false : null;
        const totalPayable = baseRateTotal + taxTotal + depositTotal;

        this.frmBooking.patchValue({
            isChargeableSnapshot,
            amountBeforeTax: hasAnyRate ? baseRateTotal : '',
            taxAmount: hasAnyTax ? taxTotal : '',
            depositAmount: hasAnyDeposit ? depositTotal : '',
            totalPayable: hasAnyRate || hasAnyDeposit || hasAnyTax ? totalPayable : ''
        }, { emitEvent: false });
    }

    private resetChargesSnapshot(): void {
        this.frmBooking.patchValue({
            isChargeableSnapshot: null,
            amountBeforeTax: '',
            taxAmount: '',
            depositAmount: '',
            totalPayable: ''
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
