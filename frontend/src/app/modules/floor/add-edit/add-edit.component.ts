import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FloorService } from '../floor.service';
import { ToastrService } from 'ngx-toastr';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';

@Component({
    selector: 'floor-add-edit',
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
                MatButtonModule, CommonModule,
    ]
})
export class FloorAddEditComponent implements OnInit {
    floorId: number;
    isEditMode = false;
    frmFloor = this.fb.group({
        // Keep visually empty and disabled; set payload on create
        code: [{ value: '', disabled: true }],
        floorName: ['', Validators.required],
        buildingId: ['', Validators.required],
        isActive: [true]
    });
    buildings: any[] = [];
    page = ApplicationPage.floor;
    permissions = PermissionType;
    IsViewPermission = false;

    constructor(private fb: FormBuilder, private floorService: FloorService, private route: ActivatedRoute, private router: Router, private notificationService: ToastrService, private permissionService: PermissionService) { }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Floor (PER_FLOOR) - View');
        this.loadBuildings();
        this.route.params.subscribe(params => {
            if (params['id']) {
                this.isEditMode = true;
                this.floorId = +params['id'];
                this.getFloorDetails();
            }
        });
    }

    loadBuildings() {
        this.floorService.getBuildings().subscribe((res: any) => {
            this.buildings = res;
        });
    }

    private getFloorDetails() {
        this.floorService.getFloorById(this.floorId).subscribe((res: any) => {
            this.frmFloor.patchValue({
                code: res.code,
                floorName: res.floorName,
                buildingId: res.buildingId,
                isActive: res.isActive
            });
        });
    }

    save() {
        const formValue = this.frmFloor.getRawValue();
        const payload = {
            ...formValue,
            // For create mode, send code as 'string' while keeping input visually empty
            code: this.isEditMode ? formValue.code : 'string',
            createdDate: new Date().toISOString(),
            createdBy: 0,
            modifiedDate: new Date().toISOString(),
            modifiedBy: 0,
            id: this.isEditMode ? this.floorId : 0
        };
        const request$ = this.isEditMode ? this.floorService.updateFloor(payload) : this.floorService.addFloor(payload);
        request$.subscribe(() => {
            this.notificationService.success('Saved successfully.');
            this.router.navigate(['/floor']);
        }, () => this.notificationService.error('Save failed.'));
    }
    cancel() {
       this.router.navigate(['floor']);
    }
}
