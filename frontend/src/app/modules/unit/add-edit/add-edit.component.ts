import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UnitService } from '../unit.service';
import { ToastrService } from 'ngx-toastr';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { FloorService } from 'app/modules/floor/floor.service';

@Component({
    selector: 'unit-add-edit',
    standalone: true,
    templateUrl: './add-edit.component.html',
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
        CommonModule,
    ]
})
export class UnitAddEditComponent implements OnInit {
    unitId: number;
    isEditMode = false;
    frmUnit = this.fb.group({
        code: [{ value: '', disabled: true }],
        unitName: ['', Validators.required],
        buildingId: ['', Validators.required],
        floorId: ['', Validators.required],
        occupancyStatusId: ['', Validators.required],
        isActive: [true]
    });
    buildings: any[] = [];
    floors: any[] = [];
    occupancyStatuses: any[] = [];
    page = ApplicationPage.unit;
    permissions = PermissionType;
    IsViewPermission = false;

    constructor(private fb: FormBuilder, private unitService: UnitService, private route: ActivatedRoute, private router: Router, private notificationService: ToastrService, private permissionService: PermissionService,
        private floorService: FloorService
    ) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Unit (PER_UNIT) - View');
        this.loadLookups();
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.unitId = +params['id'];
                this.getUnitDetails();
            }
        });
    }

    loadLookups() {
        this.unitService.getBuildings().subscribe((res: any) => this.buildings = res.items || res);
         this.floorService.getFloors().subscribe((res: any) => this.floors = res.items || res);
        this.unitService.getOccupancyStatuses().subscribe((res: any) => this.occupancyStatuses = res.items || res);
    }

    private getUnitDetails() {
        this.unitService.getUnitById(this.unitId).subscribe((res: any) => {
            this.frmUnit.patchValue({
                code: res.code,
                unitName: res.unitName,
                buildingId: res.buildingId,
                floorId: res.floorId,
                occupancyStatusId: res.occupancyStatusId,
                isActive: res.isActive
            });
        });
    }

    save() {
        const formValue = this.frmUnit.getRawValue();
        const payload = {
            ...formValue,
            buildingId: +formValue.buildingId,
            floorId: +formValue.floorId,
            occupancyStatusId: +formValue.occupancyStatusId,
            code: this.isEditMode ? formValue.code : 'string',
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.unitId : 0
        };
        const request$ = this.isEditMode ? this.unitService.updateUnit(payload) : this.unitService.addUnit(payload);
        request$.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/unit']);
        }, () => this.notificationService.error('Save failed.'));
    }
    cancel() {
       this.router.navigate(['unit']);
    }
}