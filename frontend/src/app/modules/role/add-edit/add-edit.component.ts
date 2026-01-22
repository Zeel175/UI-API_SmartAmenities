import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormGroup, UntypedFormBuilder, UntypedFormArray, Validators, AbstractControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
// import { ApplicationPage, CommonUtility } from 'app/constants';
import { ApplicationPage } from 'app/core';
import { Permission, Role } from 'app/model';
import { RoleService } from '../role.service';
import { PermissionService } from 'app/core/service/permission.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { FuseCardComponent } from '@fuse/components/card';
import { CommonUtility } from 'app/core';

@Component({
    selector: 'role-add-edit',
    templateUrl: './add-edit.component.html',
    standalone: true,
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
        MatCheckboxModule
    ]
})
export class RoleAddEditComponent {
    roleData: Role;
    roleId: number;
    isEditMode: boolean;
    frmRole: UntypedFormGroup;
    routerSub: Subscription;
    isFormSubmitted: boolean;
    page: string = ApplicationPage.role;
    error: string;

    IsViewPermission: boolean = false;

    Permissions = [
        { permission: 'Dashboard', view: false, add: false, edit: false, delete: false },
        { permission: 'User', view: false, add: false, edit: false, delete: false },
        { permission: 'Role', view: false, add: false, edit: false, delete: false },
        { permission: 'Weight Check', view: false, add: false, edit: false, delete: false },
        { permission: 'Attribute Check', view: false, add: false, edit: false, delete: false },
        { permission: 'PreCheck List', view: false, add: false, edit: false, delete: false },
        { permission: 'PostCheck List', view: false, add: false, edit: false, delete: false },
        { permission: 'Liquid Preparation', view: false, add: false, edit: false, delete: false },
        { permission: 'Trailer Loading', view: false, add: false, edit: false, delete: false },
        { permission: 'Trailer Inspection', view: false, add: false, edit: false, delete: false },
        { permission: 'Pallet Packing', view: false, add: false, edit: false, delete: false },
        { permission: 'DownTime Tracking', view: false, add: false, edit: false, delete: false },
        { permission: 'Production Order', view: false, add: false, edit: false, delete: false },
        { permission: 'Property', view: false, add: false, edit: false, delete: false },
        { permission: 'Building', view: false, add: false, edit: false, delete: false },
        { permission: 'Floor', view: false, add: false, edit: false, delete: false },
        { permission: 'Unit', view: false, add: false, edit: false, delete: false },
        { permission: 'AuditLog', view: false, add: false, edit: false, delete: false },
        { permission: 'GroupCode', view: false, add: false, edit: false, delete: false },
        { permission: 'Zone', view: false, add: false, edit: false, delete: false },
        { permission: 'Resident', view: false, add: false, edit: false, delete: false },
        { permission: 'Guest', view: false, add: false, edit: false, delete: false },
        { permission: 'Amenity', view: false, add: false, edit: false, delete: false },
        { permission: 'Amenity Slot Template', view: false, add: false, edit: false, delete: false },
        // Add more permissions as needed...
    ];

    permissionId: number;
    PermissionData: any;

    constructor(
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private formBuilder: UntypedFormBuilder,
        private roleService: RoleService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) {
        this.createForm();
    }

    ngOnInit(): void {
        this.getRoleRoute();
        this.getPermissionDetails();

        this.IsViewPermission = this.permissionService.hasPermission('Role (PER_ROLE) - View');

        // const transformedPermissions = this.transformPermissionsEdit(this.staticRolePermissions, this.staticPermissionData);
        // console.log('Transformed Permissions with Static Data:', transformedPermissions);
    }

    private getRoleRoute() {
        this.routerSub = this.activatedRoute.params.subscribe((params) => {
            this.isEditMode = !CommonUtility.isEmpty(params["id"]);
            if (this.isEditMode) {
                this.roleId = +params.id;
                this.getRoleDetails();
             //   console.log("edit called");
            }
        });
    }
    private getPermissionDetails() {
        this.roleService.getPermission(this.permissionId)
            .subscribe((result: any) => {
                this.PermissionData = result;
               // console.log("Permission Data:", this.PermissionData);
    
                if (this.roleData) {
                    const transformedPermissions = this.transformPermissionsEdit(this.roleData.rolePermissions, this.PermissionData);
                    this.setRoleData(transformedPermissions);
                }
            },
            (error) => {
                console.error(error);
                this.notificationService.error("Error fetching permission details: " + error.message);
            });
    }


    private getRoleDetails() {
        this.roleService.getRoleById(this.roleId)
            .subscribe((result: any) => {
                this.roleData = result;
             //   console.log("Role Data:", this.roleData);
    
                // Ensure roleData is fetched before fetching and transforming permissions
                if (this.roleData && this.PermissionData) {
                    const transformedPermissions = this.transformPermissionsEdit(this.roleData.rolePermissions, this.PermissionData);
                    this.setRoleData(transformedPermissions);
                }
            },
            (error) => {
                console.error(error);
                this.notificationService.error("Error fetching role details: " + error.message);
            });
    }
    createForm() {
        this.frmRole = this.formBuilder.group({
            name: ['', [Validators.required, Validators.maxLength(100)]],
            Permissions: this.formBuilder.array(
                this.Permissions.map(permission => this.createPermissionGroup(permission))
            )
        });
    }

    private transformPermissionsEdit(rolePermissions: any[], permissionData: any[]): any[] {
        // Map permission IDs to their details
        const permissionMap = new Map<number, any>();
        permissionData.forEach(permission => {
            permissionMap.set(permission.id, {
                name: permission.name,
                type: permission.permissionTypeId
            });
        });
    
       // console.log('Permission Map:', Array.from(permissionMap.entries()));
    
        // Initialize default permissions with actions set to false
        const transformedPermissions = this.Permissions.map(permission => ({
            ...permission,
            view: false,
            add: false,
            edit: false,
            delete: false
        }));
    
        // Map rolePermissions to corresponding permissions in the form
        rolePermissions.forEach(rolePerm => {
            const permissionDetail = permissionMap.get(rolePerm.permissionId);
            if (permissionDetail) {
                const perm = transformedPermissions.find(p => p.permission === permissionDetail.name);
                if (perm) {
                    // Set permission actions based on permissionTypeId
                    switch (permissionDetail.type) {
                        case 72: perm.add = true; break;
                        case 73: perm.edit = true; break;
                        case 74: perm.view = true; break;
                        case 75: perm.delete = true; break;
                    }
                }
            }
        });
    
      //  console.log('Transformed Permissions:', transformedPermissions);
        return transformedPermissions;
    }
    

    private setRoleData(transformedPermissions: any[]) {
        this.frmRole.patchValue({
            name: this.roleData.name
        });
    
        const permissionFormArray = this.frmRole.get('Permissions') as UntypedFormArray;
    
        // Clear the form array before adding new values
        permissionFormArray.clear();
    
        // Map transformedPermissions to the form array
        transformedPermissions.forEach((perm: any) => {
            permissionFormArray.push(
                this.formBuilder.group({
                    permission: [perm.permission],
                    view: [perm.view],
                    add: [perm.add],
                    edit: [perm.edit],
                    delete: [perm.delete]
                })
            );
        });
    
      //  console.log(this.frmRole.value); // Debugging: Check form value
    }

    // private createRole() {
    //     const role: Role = this.frmRole.value;
    //     const payload: Role = {
    //         id: 0, // or an appropriate value if updating
    //         name: role.name,
    //         assignedPermissions: role.permissions,
    //         permissions: []
    //     };
    //     this.roleService.addRole(payload).subscribe(
    //         () => {
    //             this.cancel();
    //             this.notificationService.success("Role created successfully.");
    //         },
    //         (error) => {
    //             this.error = error;
    //             this.notificationService.error("Error creating role: " + error.message);
    //         }
    //     );
    // }
    createRole() {
        const role: Role = this.frmRole.value;

        // Transform permissions into the required format
        const transformedPermissions = this.transformPermissions(role.Permissions);

        const payload: Role = {
            id: 0, // or an appropriate value if updating
            name: role.name,
            Permissions: transformedPermissions,
            rolePermissions:transformedPermissions, // Assign to permissions field
            assignedPermissions: [],
            NormalizedName:role.name.toUpperCase() // Leave empty or adjust as needed
        };

        this.roleService.addRole(payload).subscribe(
            () => {
                this.cancel();
                this.notificationService.success("Role created successfully.");
            },
            (error) => {
                this.error = error;
                this.notificationService.error("Error creating role: " + error.message);
            }
        );
    }
    createPermissionGroup(permission: any): UntypedFormGroup {
        return this.formBuilder.group({
            permission: [permission.permission],
            view: [permission.view],
            add: [permission.add],
            edit: [permission.edit],
            delete: [permission.delete]
        });
    }
    // private updateRole() {
    //     const role: Role = this.frmRole.value;
    //     const payload: Role = {
    //         ...this.roleData,
    //         name: role.name,
    //         assignedPermissions: role.permissions
    //     };

    //     this.roleService.updateRole(this.roleData.id, payload).subscribe(
    //         () => {
    //             this.cancel();
    //             this.notificationService.success("Role updated successfully.");
    //         },
    //         (error) => {
    //             this.error = error;
    //             this.notificationService.error("Error updating role: " + error.message);
    //         }
    //     );
    // }
    private updateRole() {
        const role: Role = this.frmRole.value;

        // Transform permissions into the required format
        const transformedPermissions = this.transformPermissions(role.Permissions);

        const payload: Role = {
            ...this.roleData,
            name: role.name,
            Permissions: transformedPermissions,
            NormalizedName:role.name.toUpperCase() // Assign to permissions field
        };

        this.roleService.updateRole(this.roleData.id, payload).subscribe(
            () => {
                this.cancel();
                this.notificationService.success("Role updated successfully.");
            },
            (error) => {
                this.error = error;
                this.notificationService.error("Error updating role: " + error.message);
            }
        );
    }
    private transformPermissions(formPermissions: any[]): any[] {
        return formPermissions.map(permission => ({
            Code: this.mapPermissionToCode(permission.permission),
            IsList: permission.view,
            IsAdd: permission.add,
            IsEdit: permission.edit,
            IsDelete: permission.delete,
            IsExport: false // Set to false or include logic if export permissions are required
        }));
    }

    private mapPermissionToCode(permissionName: string): string {
        // Map permission names to codes. Adjust this mapping as needed.
        const permissionMap = {
            'Dashboard': 'PER_DASHBOARD',
            'User': 'PER_USER',
            'Role': 'PER_ROLE',
            'Weight Check': 'PER_WEIGHTCHECK',
            'Attribute Check': 'PER_ATTRIBUTECHECK',
            'PreCheck List': 'PER_PRECHEKLIST',
            'Liquid Preparation': 'PER_LIQUIDPREPARATION',
            'Trailer Inspection':'PER_TRAILERINSPECTION',
            'PostCheck List': 'PER_POSTCHEKLIST',
            'DownTime Tracking': 'PER_DOWNTIMECHECKING',
            'Pallet Packing': 'PER_PALLETPACKING',
            'Production Order': 'PER_PURCHASEORDER',
            'Trailer Loading':'PER_TRAILERLOADING',
            'Property' : 'PER_PROPERTY',
            'Building':'PER_BUILDING',
            'Floor' : 'PER_FLOOR',
            'Unit' : 'PER_UNIT',
            'AuditLog' : 'PER_AUDITLOG',
            'GroupCode' : 'PER_GROUP_CODE',
            'Zone' : 'PER_ZONE',
            'Resident' : 'PER_RESIDENT',
            'Guest' : 'PER_GUEST',
            'Amenity' : 'PER_AMENITY',
            'Amenity Slot Template' : 'PER_AMENITY_SLOT_TEMPLATE'
        };

        return permissionMap[permissionName] || 'UNKNOWN_CODE'; // Default to UNKNOWN_CODE if not found
    }
    save() {
        this.isFormSubmitted = true;
        if (this.frmRole.invalid) {
            this.notificationService.error("Please fill all required fields.");
            return;
        }

        if (this.isEditMode) {
            this.updateRole();
        } else {
            this.createRole();
        }
    }

    cancel() {
       this.router.navigate(['role']);
    }

    ngOnDestroy(): void {
        if (this.routerSub) {
            this.routerSub.unsubscribe();
        }
    }
}
