import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, CommonUtility, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { ToastrService } from 'ngx-toastr';
import { AmenityDocument } from 'app/model';
import { AmenityMasterService } from '../amenity-master.service';

@Component({
    selector: 'amenity-master-add-edit',
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
        MatCheckboxModule,
        NgxMaterialTimepickerModule,
        CommonModule
    ]
})
export class AmenityMasterAddEditComponent implements OnInit {
    amenityId: number;
    isEditMode = false;
    daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    frmAmenity = this.fb.group({
        code: [{ value: '', disabled: true }],
        name: ['', Validators.required],
        type: ['', Validators.required],
        description: [''],
        buildingId: [null],
        floorId: [null],
        deviceId: [null],
        deviceUserName: [''],
        devicePassword: [''],
        location: [''],
        status: ['Active', Validators.required],
        maxCapacity: [''],
        maxBookingsPerDayPerFlat: [''],
        maxActiveBookingsPerFlat: [''],
        minAdvanceBookingHours: [''],
        minAdvanceBookingDays: [''],
        maxAdvanceBookingDays: [''],
        bookingSlotRequired: [false],
        slotDurationMinutes: [''],
        bufferTimeMinutes: [''],
        allowMultipleSlotsPerBooking: [false],
        allowMultipleUnits: [false],
        requiresApproval: [false],
        allowGuests: [false],
        maxGuestsAllowed: [''],
        availableDays: this.fb.control(this.daysOfWeek),
        openTime: [''],
        closeTime: [''],
        holidayBlocked: [false],
        maintenanceSchedule: [''],
        isChargeable: [false],
        chargeType: ['Free'],
        baseRate: [''],
        securityDeposit: [''],
        refundableDeposit: [false],
        taxApplicable: [false],
        taxCodeId: [''],
        taxPercentage: [''],
        termsAndConditions: ['']
    });

    buildings: any[] = [];
    floors: any[] = [];
    devices: any[] = [];
    types = ['Indoor', 'Outdoor', 'Room', 'Court', 'Service'];
    statuses = ['Active', 'Inactive', 'Maintenance'];
    yesNoOptions = [
        { label: 'Yes', value: true },
        { label: 'No', value: false }
    ];
    chargeTypes = ['Per Slot', 'Per Hour', 'Per Day', 'Flat Fee'];
    page = ApplicationPage.amenityMaster;
    permissions = PermissionType;
    IsViewPermission = false;
    selectedDocuments: File[] = [];
    existingDocuments: AmenityDocument[] = [];

    constructor(
        private fb: FormBuilder,
        private amenityService: AmenityMasterService,
        private route: ActivatedRoute,
        private router: Router,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Amenity (PER_AMENITY) - View');
        this.loadLookups();
        this.frmAmenity.get('buildingId')?.valueChanges.subscribe(() => {
            this.updateDeviceCredentialsState();
        });
        this.frmAmenity.get('allowMultipleUnits')?.valueChanges.subscribe(() => {
            this.updateDeviceCredentialsState();
        });
        this.frmAmenity.get('deviceId')?.valueChanges.subscribe(() => {
            this.updateDeviceCredentialsState();
        });
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.amenityId = +params['id'];
                this.getAmenityDetails();
            }
        });
    }

    loadLookups(): void {
        this.amenityService.getBuildings().subscribe((res: any) => {
            this.buildings = res.items || res;
        });
        this.amenityService.getFloors().subscribe((res: any) => {
            this.floors = res.items || res;
        });
        this.amenityService.getHikvisionDevices().subscribe((res: any) => {
            this.devices = res.items || res;
        }, () => this.notificationService.error('Failed to load device list.'));
    }

    private getAmenityDetails(): void {
        this.amenityService.getAmenityById(this.amenityId).subscribe((res: any) => {
            this.frmAmenity.patchValue({
                code: res.code,
                name: res.name,
                type: res.type,
                description: res.description,
                buildingId: res.buildingId,
                floorId: res.floorId,
                deviceId: res.deviceId,
                deviceUserName: res.deviceUserName,
                devicePassword: res.devicePassword,
                location: res.location,
                status: res.status,
                maxCapacity: res.maxCapacity,
                maxBookingsPerDayPerFlat: res.maxBookingsPerDayPerFlat,
                maxActiveBookingsPerFlat: res.maxActiveBookingsPerFlat,
                minAdvanceBookingHours: res.minAdvanceBookingHours,
                minAdvanceBookingDays: res.minAdvanceBookingDays,
                maxAdvanceBookingDays: res.maxAdvanceBookingDays,
                bookingSlotRequired: !!res.bookingSlotRequired,
                slotDurationMinutes: res.slotDurationMinutes,
                bufferTimeMinutes: res.bufferTimeMinutes,
                allowMultipleSlotsPerBooking: !!res.allowMultipleSlotsPerBooking,
                allowMultipleUnits: !!res.allowMultipleUnits,
                requiresApproval: !!res.requiresApproval,
                allowGuests: !!res.allowGuests,
                maxGuestsAllowed: res.maxGuestsAllowed,
                availableDays: res.availableDays ? res.availableDays.split(',').map((day: string) => day.trim()).filter(Boolean) : [],
                openTime: this.formatTimeForPicker(res.openTime),
                closeTime: this.formatTimeForPicker(res.closeTime),
                holidayBlocked: !!res.holidayBlocked,
                maintenanceSchedule: res.maintenanceSchedule,
                isChargeable: !!res.isChargeable,
                chargeType: res.chargeType || 'Free',
                baseRate: res.baseRate,
                securityDeposit: res.securityDeposit,
                refundableDeposit: !!res.refundableDeposit,
                taxApplicable: !!res.taxApplicable,
                taxCodeId: res.taxCodeId,
                taxPercentage: res.taxPercentage,
                termsAndConditions: res.termsAndConditions
            });
            this.existingDocuments = res.documentDetails ?? [];
            this.updateDeviceCredentialsState();
        });
    }

    onDocumentsSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (input.files && input.files.length > 0) {
            Array.from(input.files).forEach((file) => {
                this.selectedDocuments.push(file);
            });
        }

        input.value = '';
    }

    removeSelectedDocument(index: number): void {
        this.selectedDocuments.splice(index, 1);
    }

    removeExistingDocument(document: AmenityDocument, index: number): void {
        if (!document?.id) {
            return;
        }

        if (!confirm('Remove this document?')) {
            return;
        }

        this.amenityService.deleteAmenityDocument(document.id).subscribe({
            next: () => {
                this.existingDocuments.splice(index, 1);
                this.notificationService.success('Document removed.');
            },
            error: () => this.notificationService.error('Failed to remove document.')
        });
    }

    openSelectedDocument(file: File): void {
        const fileUrl = URL.createObjectURL(file);
        window.open(fileUrl, '_blank');
        setTimeout(() => {
            URL.revokeObjectURL(fileUrl);
        }, 1000);
    }

    save(): void {
        const formValue = this.frmAmenity.getRawValue();
        const hasBuilding = !CommonUtility.isEmpty(formValue.buildingId);
        const allowMultipleUnits = !!formValue.allowMultipleUnits;
        const shouldClearDevice = hasBuilding || allowMultipleUnits;
        const payload = {
            ...formValue,
            buildingId: this.toNumber(formValue.buildingId),
            floorId: this.toNumber(formValue.floorId),
            deviceId: shouldClearDevice ? null : this.toNumber(formValue.deviceId),
            deviceUserName: shouldClearDevice ? null : this.toNullableString(formValue.deviceUserName),
            devicePassword: shouldClearDevice ? null : this.toNullableString(formValue.devicePassword),
            maxCapacity: this.toNumber(formValue.maxCapacity),
            maxBookingsPerDayPerFlat: this.toNumber(formValue.maxBookingsPerDayPerFlat),
            maxActiveBookingsPerFlat: this.toNumber(formValue.maxActiveBookingsPerFlat),
            minAdvanceBookingHours: this.toNumber(formValue.minAdvanceBookingHours),
            minAdvanceBookingDays: this.toNumber(formValue.minAdvanceBookingDays),
            maxAdvanceBookingDays: this.toNumber(formValue.maxAdvanceBookingDays),
            slotDurationMinutes: this.toNumber(formValue.slotDurationMinutes),
            bufferTimeMinutes: this.toNumber(formValue.bufferTimeMinutes),
            maxGuestsAllowed: this.toNumber(formValue.maxGuestsAllowed),
            availableDays: Array.isArray(formValue.availableDays)
                ? formValue.availableDays.join(',')
                : (formValue.availableDays ?? ''),
            openTime: this.formatTimeForPayload(formValue.openTime),
            closeTime: this.formatTimeForPayload(formValue.closeTime),
            baseRate: this.toNumber(formValue.baseRate),
            securityDeposit: this.toNumber(formValue.securityDeposit),
            taxCodeId: this.toNumber(formValue.taxCodeId),
            taxPercentage: this.toNumber(formValue.taxPercentage),
            code: this.isEditMode ? formValue.code : 'string',
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.amenityId : 0
        };

        const formData = this.buildFormData(payload);
        this.selectedDocuments.forEach((file) => formData.append('Documents', file));

        const request$ = this.isEditMode
            ? this.amenityService.updateAmenity(formData)
            : this.amenityService.addAmenity(formData);

        request$.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/amenity-master']);
        }, () => this.notificationService.error('Save failed.'));
    }

    cancel(): void {
        this.router.navigate(['amenity-master']);
    }

    private toNumber(value: unknown): number | null {
        if (value === null || value === undefined || value === '') {
            return null;
        }
        const parsed = Number(value);
        return Number.isNaN(parsed) ? null : parsed;
    }

    private formatTimeForPicker(value: string | null | undefined): string {
        if (!value) {
            return '';
        }
        const parsed = this.parseTimeString(value);
        if (!parsed) {
            return '';
        }
        return this.formatTimeToMeridiem(parsed.hours, parsed.minutes);
    }

    private formatTimeForPayload(value: string | null | undefined): string | null {
        if (!value) {
            return null;
        }
        const parsed = this.parseTimeString(value);
        if (!parsed) {
            return null;
        }
        const hours = parsed.hours.toString().padStart(2, '0');
        const minutes = parsed.minutes.toString().padStart(2, '0');
        return `${hours}:${minutes}`;
    }

    private formatTimeToMeridiem(hours: number, minutes: number): string {
        const meridiem = hours >= 12 ? 'PM' : 'AM';
        const normalized = hours % 12 === 0 ? 12 : hours % 12;
        return `${normalized}:${minutes.toString().padStart(2, '0')} ${meridiem}`;
    }

    private parseTimeString(time: string): { hours: number; minutes: number } | null {
        const value = (time || '').trim();
        if (!value) {
            return null;
        }

        const match = value.match(/^\s*(\d{1,2})(?::(\d{1,2}))?(?::(\d{1,2}))?(?:\.(\d+))?\s*(AM|PM)?\s*$/i);
        if (!match) {
            return null;
        }

        let hours = Number(match[1]);
        const minutes = Number(match[2] ?? '0');
        const meridiem = (match[5] ?? '').toUpperCase();

        if (!Number.isFinite(hours) || !Number.isFinite(minutes)) {
            return null;
        }
        if (minutes < 0 || minutes > 59) {
            return null;
        }

        if (meridiem === 'AM' || meridiem === 'PM') {
            if (hours < 1 || hours > 12) {
                return null;
            }
            if (hours === 12) {
                hours = meridiem === 'AM' ? 0 : 12;
            } else if (meridiem === 'PM') {
                hours += 12;
            }
        } else if (hours < 0 || hours > 23) {
            return null;
        }

        return { hours, minutes };
    }

    private toNullableString(value: unknown): string | null {
        if (value === null || value === undefined) {
            return null;
        }
        const normalized = String(value).trim();
        return normalized.length ? normalized : null;
    }

    private toggleDeviceCredentialsValidators(hasDevice: boolean) {
        const userNameControl = this.frmAmenity.get('deviceUserName');
        const passwordControl = this.frmAmenity.get('devicePassword');
        if (!userNameControl || !passwordControl) {
            return;
        }
        if (hasDevice) {
            userNameControl.setValidators([Validators.required]);
            passwordControl.setValidators([Validators.required]);
        } else {
            userNameControl.clearValidators();
            passwordControl.clearValidators();
            userNameControl.setValue('');
            passwordControl.setValue('');
        }
        userNameControl.updateValueAndValidity();
        passwordControl.updateValueAndValidity();
    }

    private updateDeviceCredentialsState(): void {
        const hasBuilding = !CommonUtility.isEmpty(this.frmAmenity.get('buildingId')?.value);
        const allowMultipleUnits = !!this.frmAmenity.get('allowMultipleUnits')?.value;
        const hasDevice = !CommonUtility.isEmpty(this.frmAmenity.get('deviceId')?.value);

        if (hasBuilding || allowMultipleUnits) {
            this.frmAmenity.patchValue({
                deviceId: null,
                deviceUserName: '',
                devicePassword: ''
            }, { emitEvent: false });
            this.toggleDeviceCredentialsValidators(false);
            return;
        }

        this.toggleDeviceCredentialsValidators(hasDevice);
    }

    private buildFormData(payload: Record<string, unknown>): FormData {
        const formData = new FormData();
        Object.entries(payload).forEach(([key, value]) => {
            if (value === null || value === undefined) {
                return;
            }
            if (Array.isArray(value)) {
                value.forEach((item) => {
                    if (item !== null && item !== undefined) {
                        formData.append(key, String(item));
                    }
                });
                return;
            }
            formData.append(key, String(value));
        });
        return formData;
    }
}
