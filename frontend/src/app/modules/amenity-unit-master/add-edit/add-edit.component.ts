import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { AmenityUnit } from 'app/model';
import { ToastrService } from 'ngx-toastr';
import { QuillModule } from 'ngx-quill';
import { AmenityUnitMasterService } from '../amenity-unit-master.service';

@Component({
    selector: 'amenity-unit-master-add-edit',
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
        MatChipsModule,
        QuillModule,
        CommonModule
    ]
})
export class AmenityUnitMasterAddEditComponent implements OnInit {
    unitId: number;
    isEditMode = false;
    amenities: any[] = [];
    allAmenities: any[] = [];
    devices: any[] = [];
    selectedAmenityId: number | null = null;
    statuses = ['Active', 'Inactive', 'Maintenance'];
    chargeTypes = ['Free', 'Per Slot', 'Per Hour', 'Per Day', 'Flat Fee'];
    yesNoOptions = [
        { label: 'Yes', value: true },
        { label: 'No', value: false }
    ];

    featureName = '';
    quillModules = {
        toolbar: [
            ['bold', 'italic', 'underline', 'strike'],
            [{ script: 'sub' }, { script: 'super' }],
            [{ header: 1 }, { header: 2 }],
            [{ list: 'ordered' }, { list: 'bullet' }],
            [{ align: [] }],
            [{ color: [] }, { background: [] }],
            ['link'],
            ['clean']
        ]
    };

    frmAmenityUnit = this.fb.group({
        amenityId: ['', Validators.required],
        unitName: ['', Validators.required],
        unitCode: [{ value: '', disabled: true }],
        deviceId: [null],
        deviceUserName: [''],
        devicePassword: [''],
        status: ['Active', Validators.required],
        shortDescription: [''],
        longDescription: [''],
        isChargeable: [false],
        chargeType: [''],
        baseRate: [null],
        securityDeposit: [null],
        refundableDeposit: [false],
        taxApplicable: [false],
        taxCodeId: [null],
        taxPercentage: [null],
        features: this.fb.array([])
    });

    page = ApplicationPage.amenityUnitMaster;
    IsViewPermission = false;

    constructor(
        private fb: FormBuilder,
        private amenityUnitService: AmenityUnitMasterService,
        private route: ActivatedRoute,
        private router: Router,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Amenity Unit (PER_AMENITY_UNIT) - View');
        this.loadAmenities();
        this.loadDevices();
        this.frmAmenityUnit.get('deviceId')?.valueChanges.subscribe((deviceId) => {
            const hasDevice = deviceId !== null && deviceId !== undefined && deviceId !== '';
            this.toggleDeviceCredentialsValidators(hasDevice);
        });
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.unitId = +params['id'];
                this.getAmenityUnitDetails();
            }
        });
    }

    get features(): FormArray {
        return this.frmAmenityUnit.get('features') as FormArray;
    }

    private loadAmenities(): void {
        this.amenityUnitService.getAmenities().subscribe((res: any) => {
            this.allAmenities = res.items || res;
            this.filterAmenities();
        });
    }

    private loadDevices(): void {
        this.amenityUnitService.getHikvisionDevices().subscribe((res: any) => {
            this.devices = res.items || res;
        }, () => this.notificationService.error('Failed to load device list.'));
    }

    private filterAmenities(): void {
        this.amenities = (this.allAmenities || []).filter((amenity: any) => {
            if (amenity?.allowMultipleUnits) {
                return true;
            }
            return this.selectedAmenityId !== null && amenity?.id === this.selectedAmenityId;
        });
    }

    private getAmenityUnitDetails(): void {
        this.amenityUnitService.getAmenityUnitById(this.unitId).subscribe((res: any) => {
            this.selectedAmenityId = res.amenityId ?? null;
            this.frmAmenityUnit.patchValue({
                amenityId: res.amenityId,
                unitName: res.unitName,
                unitCode: res.unitCode,
                deviceId: res.deviceId,
                deviceUserName: res.deviceUserName,
                devicePassword: res.devicePassword,
                status: res.status,
                shortDescription: res.shortDescription,
                longDescription: res.longDescription,
                isChargeable: res.isChargeable,
                chargeType: res.chargeType,
                baseRate: res.baseRate,
                securityDeposit: res.securityDeposit,
                refundableDeposit: res.refundableDeposit,
                taxApplicable: res.taxApplicable,
                taxCodeId: res.taxCodeId,
                taxPercentage: res.taxPercentage
            });
            this.filterAmenities();
            this.toggleDeviceCredentialsValidators(!!res.deviceId);

            this.features.clear();
            (res.features || []).forEach((feature: any) => {
                this.features.push(this.fb.group({
                    featureId: [feature.featureId],
                    featureName: [feature.featureName, Validators.required],
                    isActive: [feature.isActive ?? true]
                }));
            });
        });
    }

    addFeature(): void {
        const name = (this.featureName || '').trim();
        if (!name) {
            this.notificationService.error('Feature name is required.');
            return;
        }

        this.features.push(this.fb.group({
            featureId: [null],
            featureName: [name, Validators.required],
            isActive: [true]
        }));
        this.featureName = '';
    }

    removeFeature(index: number): void {
        this.features.removeAt(index);
    }

    save(): void {
        if (this.frmAmenityUnit.invalid) {
            this.frmAmenityUnit.markAllAsTouched();
            this.notificationService.error('Please fill all required fields.');
            return;
        }

        const formValue = this.frmAmenityUnit.getRawValue();
        const payload: AmenityUnit = {
            id: this.isEditMode ? this.unitId : 0,
            amenityId: +formValue.amenityId,
            unitName: formValue.unitName,
            unitCode: this.isEditMode ? formValue.unitCode : null,
            deviceId: formValue.deviceId ? +formValue.deviceId : null,
            deviceUserName: formValue.deviceId ? formValue.deviceUserName : null,
            devicePassword: formValue.deviceId ? formValue.devicePassword : null,
            status: formValue.status,
            shortDescription: formValue.shortDescription,
            longDescription: formValue.longDescription,
            isChargeable: formValue.isChargeable,
            chargeType: formValue.chargeType,
            baseRate: formValue.baseRate,
            securityDeposit: formValue.securityDeposit,
            refundableDeposit: formValue.refundableDeposit,
            taxApplicable: formValue.taxApplicable,
            taxCodeId: formValue.taxCodeId,
            taxPercentage: formValue.taxPercentage,
            features: (formValue.features || []).map((feature: any) => ({
                featureId: feature.featureId,
                featureName: feature.featureName,
                isActive: feature.isActive
            }))
        };

        const saveRequest = this.isEditMode
            ? this.amenityUnitService.updateAmenityUnit(payload)
            : this.amenityUnitService.addAmenityUnit(payload);

        saveRequest.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/amenity-unit-master']);
        }, () => this.notificationService.error('Save failed.'));
    }

    private toggleDeviceCredentialsValidators(hasDevice: boolean): void {
        const userNameControl = this.frmAmenityUnit.get('deviceUserName');
        const passwordControl = this.frmAmenityUnit.get('devicePassword');
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

    cancel(): void {
        this.router.navigate(['/amenity-unit-master']);
    }
}
