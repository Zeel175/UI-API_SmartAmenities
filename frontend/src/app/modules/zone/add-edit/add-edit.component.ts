import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, CommonUtility, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { Zone } from 'app/model';
import { ZoneService } from '../zone.service';

@Component({
    selector: 'zone-add-edit',
    standalone: true,
    templateUrl: './add-edit.component.html',
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
export class ZoneAddEditComponent implements OnInit {
    zoneData: Zone;
    zoneId: number;
    isEditMode = false;
    isFormSubmitted = false;
    page: string = ApplicationPage.zone;
    permissions = PermissionType;
    IsViewPermission = false;
    buildings: any[] = [];

    frmZone = this.fb.group({
        code: [{ value: '', disabled: true }],
        zoneName: ['', Validators.required],
        buildingId: [''],
        description: [''],
        isActive: [true]
    });

    constructor(
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private fb: FormBuilder,
        private zoneService: ZoneService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Zone (PER_ZONE) - View');
        this.loadBuildings();
        this.handleRoute();
    }

    private handleRoute(): void {
        this.activatedRoute.params.subscribe((params) => {
            this.isEditMode = !CommonUtility.isEmpty(params['id']);
            if (this.isEditMode) {
                this.zoneId = +params['id'];
                this.getZoneDetails();
            } else {
                this.zoneData = {} as Zone;
            }
        });
    }

    private loadBuildings(): void {
        this.zoneService.getBuildings().subscribe((res: any) => {
            this.buildings = res.items ?? res ?? [];
        });
    }

    private getZoneDetails(): void {
        this.zoneService.getZoneById(this.zoneId).subscribe({
            next: (result: any) => {
                this.zoneData = result;
                this.frmZone.patchValue({
                    code: result.code,
                    zoneName: result.zoneName,
                    buildingId: result.buildingId,
                    description: result.description,
                    isActive: result.isActive
                });
            },
            error: () => this.notificationService.error('Failed to load zone details.')
        });
    }

    save(): void {
        if (this.frmZone.invalid) {
            this.frmZone.markAllAsTouched();
            return;
        }

        const formValue = this.frmZone.getRawValue();
        const payload: Zone = {
            ...this.zoneData,
            ...formValue,
            buildingId: +formValue.buildingId,
            code: this.isEditMode ? formValue.code : 'string',
            id: this.isEditMode ? this.zoneId : 0
        };

        const request$ = this.isEditMode ? this.zoneService.updateZone(payload) : this.zoneService.addZone(payload);
        request$.subscribe({
            next: () => {
                this.notificationService.success(this.isEditMode ? 'Zone updated successfully.' : 'Zone saved successfully.');
                this.router.navigate(['/zone']);
            },
            error: () => this.notificationService.warning('Failed to save zone.')
        });
    }

    cancel(): void {
        this.router.navigate(['/zone']);
    }
}
