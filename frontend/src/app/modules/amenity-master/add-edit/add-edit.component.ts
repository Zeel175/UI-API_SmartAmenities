import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
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
        NgxMaterialTimepickerModule,
        CommonModule
    ]
})
export class AmenityMasterAddEditComponent implements OnInit {
    amenityId: number;
    isEditMode = false;
    daysOfWeek = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    frmAmenity = this.fb.group({
        name: ['', Validators.required],
        type: ['', Validators.required],
        description: [''],
        buildingId: ['', Validators.required],
        floorId: ['', Validators.required],
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
    types = ['Indoor', 'Outdoor', 'Room', 'Court', 'Service'];
    statuses = ['Active', 'Inactive', 'Maintenance'];
    yesNoOptions = [
        { label: 'Yes', value: true },
        { label: 'No', value: false }
    ];
    chargeTypes = ['Free', 'Per Slot', 'Per Hour', 'Per Day', 'Flat Fee'];
    page = ApplicationPage.amenityMaster;
    permissions = PermissionType;
    IsViewPermission = false;
    documentFiles: File[] = [];
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
    }

    private getAmenityDetails(): void {
        this.amenityService.getAmenityById(this.amenityId).subscribe((res: any) => {
            this.frmAmenity.patchValue({
                name: res.name,
                type: res.type,
                description: res.description,
                buildingId: res.buildingId,
                floorId: res.floorId,
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
        });
    }

    onDocumentsSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        const files = input.files ? Array.from(input.files) : [];
        this.documentFiles = files;
    }

    save(): void {
        const formValue = this.frmAmenity.getRawValue();
        const payload = {
            ...formValue,
            buildingId: +formValue.buildingId,
            floorId: +formValue.floorId,
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
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.amenityId : 0
        };

        const formData = this.buildFormData(payload);
        this.documentFiles.forEach((file) => formData.append('Documents', file));

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

        const match = value.match(/^\s*(\d{1,2})(?::(\d{1,2}))?\s*(AM|PM)?\s*$/i);
        if (!match) {
            return null;
        }

        let hours = Number(match[1]);
        const minutes = Number(match[2] ?? '0');
        const meridiem = (match[3] ?? '').toUpperCase();

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
