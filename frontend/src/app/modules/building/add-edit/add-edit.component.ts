import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, CommonUtility, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { Building, Property } from 'app/model';
import { PropertyService } from 'app/modules/property/property.service';
import { ToastrService } from 'ngx-toastr';
import { BuildingService } from '../building.service';

@Component({
    selector: 'building-add-edit',
    templateUrl: './add-edit.component.html',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        FormsModule,
        RouterModule,
        MatFormFieldModule,
        MatCardModule,
        MatInputModule,
        MatIconModule,
        MatSelectModule,
        MatButtonModule,
        CommonModule,
    ]
})
export class BuildingAddEditComponent {
    buildingData: Building;
    buildingId: number;
    isEditMode: boolean;
    frmBuilding: FormGroup;
    isFormSubmitted: boolean;
    error: string;
    page: string = ApplicationPage.building;
    permissions = PermissionType;
    properties: Property[] = [];
    devices: any[] = [];
    IsViewPermission: boolean = false;

    constructor(
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private fb: FormBuilder,
        private buildingService: BuildingService,
        private propertyService: PropertyService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) {
        this.createForm();
    }

    ngOnInit(): void {
        this.getBuildingRoute();
        this.loadProperties();
        this.loadDevices();
        this.IsViewPermission = this.permissionService.hasPermission('Building (PER_BUILDING) - View');
        this.frmBuilding.get('deviceId')?.valueChanges.subscribe((deviceId) => {
            const hasDevice = !CommonUtility.isEmpty(deviceId);
            this.toggleDeviceCredentialsValidators(hasDevice);
        });
    }

    loadProperties(): void {
        this.propertyService.getAllPropertyBasic().subscribe((result: any) => {
            this.properties = result.items || result;
        });
    }
    loadDevices(): void {
        this.buildingService.getHikvisionDevices().subscribe((result: any) => {
            this.devices = result.items || result;
        }, () => {
            this.notificationService.error('Failed to load device list.');
        });
    }

    private getBuildingRoute() {
        this.activatedRoute.params.subscribe((params) => {
            this.isEditMode = !CommonUtility.isEmpty(params['id']);
            if (this.isEditMode) {
                this.buildingId = +params['id'];
                this.getBuildingDetails();
            } else {
                this.buildingData = new Building();
            }
        });
    }

    private getBuildingDetails() {
        this.buildingService.getBuildingById(this.buildingId)
            .subscribe((result: any) => {
                this.buildingData = result;
                this.setBuildingData();
            }, () => {
                this.notificationService.error('Failed to load building details.');
            });
    }

    private setBuildingData() {
        this.frmBuilding.patchValue({
            code: this.buildingData.code,
            buildingName: this.buildingData.buildingName,
            propertyId: this.buildingData.propertyId,
            deviceId: this.buildingData.deviceId,
            deviceUserName: this.buildingData.deviceUserName,
            devicePassword: this.buildingData.devicePassword,
            isActive: this.buildingData.isActive
        });
        this.toggleDeviceCredentialsValidators(!CommonUtility.isEmpty(this.buildingData.deviceId));
    }

    createForm() {
        this.frmBuilding = this.fb.group({
            // Keep visually empty, disabled; set payload in save()
            code: [{ value: '', disabled: true }],
            buildingName: ['', [Validators.required]],
            propertyId: [null, Validators.required],
            deviceId: [null, Validators.required],
            deviceUserName: [''],
            devicePassword: [''],
            isActive: [true]
        });
    }

    private toggleDeviceCredentialsValidators(hasDevice: boolean) {
        const userNameControl = this.frmBuilding.get('deviceUserName');
        const passwordControl = this.frmBuilding.get('devicePassword');
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

    save() {
        if (this.frmBuilding.invalid) {
            this.frmBuilding.markAllAsTouched();
            return;
        }
        const formValue = this.frmBuilding.getRawValue();
        let building: Building = Object.assign(this.buildingData || {}, formValue);
        // For create mode, send code as 'string' without showing it in UI
        if (!this.isEditMode) {
            building.code = 'string';
        }
        if (this.isEditMode) {
            this.buildingService.updateBuilding(building).subscribe((result: any) => {
                if (!this.handleSaveResponse(result, 'Failed to update building.')) {
                    return;
                }
                this.notificationService.success('Building updated successfully.');
                this.router.navigate(['building']);
            }, (error) => this.showSaveError(error, 'Failed to update building.'));
        } else {
            this.buildingService.addBuilding(building).subscribe((result: any) => {
                if (!this.handleSaveResponse(result, 'Failed to save building.')) {
                    return;
                }
                this.notificationService.success('Building saved successfully.');
                this.router.navigate(['building']);
            }, (error) => this.showSaveError(error, 'Failed to save building.'));
        }
    }

    private handleSaveResponse(result: any, fallbackMessage: string): boolean {
        const responseCode = result?.code?.toString();
        if ((responseCode && responseCode !== '200') || (result?.id === 0 && result?.message)) {
            this.notificationService.error(result?.message || fallbackMessage);
            return false;
        }
        return true;
    }

    private showSaveError(error: any, fallbackMessage: string) {
        const message = error?.error?.message || error?.message || fallbackMessage;
        this.notificationService.error(message);
    }

    cancel() {
        this.router.navigate(['building']);
    }
}
