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
import { MatCheckboxModule } from '@angular/material/checkbox';
import { AmenitySlotTemplateService } from '../amenity-slot-template.service';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@Component({
    selector: 'amenity-slot-template-add-edit',
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
export class AmenitySlotTemplateAddEditComponent implements OnInit {
    templateId: number;
    isEditMode = false;
    amenities: any[] = [];
    daysOfWeek = [
        'Monday',
        'Tuesday',
        'Wednesday',
        'Thursday',
        'Friday',
        'Saturday',
        'Sunday'
    ];

    frmSlotTemplate = this.fb.group({
        amenityId: ['', Validators.required],
        dayOfWeek: ['', Validators.required],
        startTime: ['', Validators.required],
        endTime: ['', Validators.required],
        slotDurationMinutes: [null, [Validators.required, Validators.min(1)]],
        bufferTimeMinutes: [null],
        capacityPerSlot: [null],
        isActive: [true]
    });

    page = ApplicationPage.amenitySlotTemplate;
    permissions = PermissionType;
    IsViewPermission = false;

    constructor(
        private fb: FormBuilder,
        private slotTemplateService: AmenitySlotTemplateService,
        private route: ActivatedRoute,
        private router: Router,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Amenity Slot Template (PER_AMENITY_SLOT_TEMPLATE) - View');
        this.loadAmenities();
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.templateId = +params['id'];
                this.getTemplateDetails();
            }
        });
    }

    private loadAmenities(): void {
        this.slotTemplateService.getAmenities().subscribe((res: any) => {
            this.amenities = res.items || res;
        });
    }

    private getTemplateDetails(): void {
        this.slotTemplateService.getSlotTemplateById(this.templateId).subscribe((res: any) => {
            this.frmSlotTemplate.patchValue({
                amenityId: res.amenityId,
                dayOfWeek: res.dayOfWeek,
                startTime: this.formatTimeForPicker(res.startTime),
                endTime: this.formatTimeForPicker(res.endTime),
                slotDurationMinutes: res.slotDurationMinutes,
                bufferTimeMinutes: res.bufferTimeMinutes,
                capacityPerSlot: res.capacityPerSlot,
                isActive: res.isActive
            });
        });
    }

    save(): void {
        const formValue = this.frmSlotTemplate.getRawValue();
        const payload = {
            ...formValue,
            amenityId: +formValue.amenityId,
            startTime: this.formatTimeForPayload(formValue.startTime),
            endTime: this.formatTimeForPayload(formValue.endTime),
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.templateId : 0
        };

        const request$ = this.isEditMode
            ? this.slotTemplateService.updateSlotTemplate(payload)
            : this.slotTemplateService.addSlotTemplate(payload);

        request$.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/amenity-slot-template']);
        }, () => this.notificationService.error('Save failed.'));
    }

    cancel(): void {
        this.router.navigate(['/amenity-slot-template']);
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

    private formatTimeForPayload(value: string | null | undefined): string {
        if (!value) {
            return '';
        }
        const parsed = this.parseTimeString(value);
        if (!parsed) {
            return '';
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
}
