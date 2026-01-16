import { Component, OnInit, OnDestroy } from '@angular/core';
import { AbstractControl, FormsModule, ReactiveFormsModule, UntypedFormArray, UntypedFormBuilder, UntypedFormControl, UntypedFormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { List } from 'app/model';
import { User } from 'app/model';
// import { ApplicationPage, CommonUtility, ListService, PermissionType } from 'app/shared';
import { ApplicationPage, CommonUtility, ListService, PermissionType } from 'app/core';
import { MatValidationMessage, ValidationService } from 'app/shared';
import { UserService } from '../user.service';
import { RoleService } from '../../role/role.service';
import { PermissionService } from 'app/core/service/permission.service';
import { FileUploadService } from 'app/shared/components/file-upload/file-upload.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { RouterModule } from '@angular/router';
import { NgxMaskDirective, provideNgxMask } from 'ngx-mask';

@Component({
    selector: 'user-add-edit',
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
        MatButtonModule, CommonModule, NgxMaskDirective, //MatValidationMessage,

    ],
    providers: [
        provideNgxMask()
    ]
})

export class UserAddEditComponent implements OnInit, OnDestroy {
 userData: User;
    userId: any;
    isEditMode: boolean;
    frmUser: UntypedFormGroup;
    routerSub: Subscription;
    isFormSubmitted: boolean;
    reportsToData: User[] = [];
    roleData: List[] = [];
    hidePassword = true;
    hideConfirmPassword = true;
    error: string;
    page: string = ApplicationPage.user;
    permissions = PermissionType;

    IsViewPermission: boolean = false;

    constructor(private activatedRoute: ActivatedRoute, private router: Router,
        private formBuilder: UntypedFormBuilder, private userService: UserService, private roleService: RoleService,
        private notificationService: ToastrService, private listService: ListService,
        private permissionService: PermissionService) {
        this.createForm();
    }

    ngOnInit(): void {
        this.getUserRoute();
        this.getRoleData();
        this.IsViewPermission = this.permissionService.hasPermission('User (PER_USER) - View');

    }

    private getUserRoute() {
        this.routerSub = this.activatedRoute.params.subscribe((params) => {
            //  console.log("Edit params ", params)
            this.isEditMode = !CommonUtility.isEmpty(params["id"]);
            this.createForm();
            if (this.isEditMode) {
                this.userId = params.id //+params["id"];
                this.getUserDetails();
            }
            else {
                this.userData = new User();
                this.getRoleData();
            }
        });
    }



    private getUserDetails() {
        // console.log("user id ", this.userId)
        this.userService.getById(this.userId)
            .subscribe((result: User) => {
                this.userData = result;
                this.setUserData();
                this.getRoleData();
                this.getUserRoles();

            }, (error) => {
                console.log(error);
            });
    }

    private getUserRoles() {
        this.userService.getRoles(this.userId)
            .subscribe((roles: string[]) => {
                // Assuming roles are returned as an array of strings
                this.frmUser.controls['role'].setValue(roles[0] || ''); // Set the first role or default value
            }, (error) => {
                console.log(error);
            });
    }



    private getRoleData() {

        this.roleService.getRole()
            .subscribe((result: any) => {
                this.roleData = result;
                //  console.log("ROLE:", this.roleData);

                this.createRolesControl();
            }, (error) => {
                console.log(error);

            });
    }

    getReportsToData() {

        const userId: number = CommonUtility.isNotNull(this.userId) ? this.userId : 0;
        this.userService.get()
            .subscribe((result: User[]) => {
                this.reportsToData = result;
                this.createReportsToControl();
            });
    }



    private setUserData() {
        this.frmUser.patchValue(this.userData);
        this.frmUser.controls.password.setValue('********');
        this.frmUser.controls.confirmPassword.setValue('********');
    }

    createForm() {
        this.frmUser = this.formBuilder.group({
            userName: [{ value: '', disabled: this.isEditMode }, [Validators.required, Validators.maxLength(50)]],
            password: [{ value: '', disabled: this.isEditMode }, [Validators.required, Validators.min(6), Validators.maxLength(20), ValidationService.passwordValidator]],
            confirmPassword: [{ value: '', disabled: this.isEditMode }, [Validators.required, ValidationService.comparePassword]],
            // firstName: ['', [Validators.required, Validators.maxLength(50)]],
            // lastName: ['', [Validators.required, Validators.maxLength(50)]],
            email: ['', [Validators.required, ValidationService.emailValidator, ValidationService.multipleemailrestrictValidator, Validators.maxLength(50)]],
            phoneNumber: ['', [Validators.maxLength(15)]],
            role: ['', [Validators.required]],
            name: ['', [Validators.required]],
            // reportsTo: new UntypedFormArray([], ValidationService.minSelectedCheckboxes(0)),
            // roles: new UntypedFormArray([], ValidationService.minSelectedCheckboxes(1)),
        });
    }

    private createReportsToControl() {
        const reportsToArray = <UntypedFormArray>this.frmUser.controls['reportsTo'];
        reportsToArray.controls = [];
        this.reportsToData.forEach(item => {
            reportsToArray.push(new UntypedFormControl((this.userData.reportsTo || []).some(x => x.reportToId === item.id)));
        });
    }

    private createRolesControl() {
        // const rolesArray = <UntypedFormArray>this.frmUser.controls['roles'];
        // rolesArray.controls = [];
        // this.roleData.forEach(item => {
        //     rolesArray.push(new UntypedFormControl((this.userData.roles || []).some(x => x.roleId === item.id)));
        // });
    }

    private createUser() {
        let user: User = Object.assign({}, this.frmUser.value);

        this.userService.add(user).subscribe(
            (result: any) => {
                if (result) {
                    this.userId = result.id; // Assign the userId from the response

                    // Now assign the role after getting the userId
                    const selectedRole = this.frmUser.get('role').value;
                    if (selectedRole) {
                        this.assignRoleToUser(this.userId, selectedRole);
                    }

                    this.notificationService.success("User saved successfully.");
                    this.cancel();
                } else {
                    this.notificationService.warning(result.message);
                }
            },
            (error) => {
                if (error.status === 400) {
                    if (Array.isArray(error.error) && error.error.length > 0) {
                        // Show the first error message from the array
                        const errorMessage = error.error[0].description;
                        this.notificationService.error(errorMessage);
                        //  this.error = error.error.modelState[''][0];
                    }
                } else {
                    this.error = 'Something went wrong';
                }
            }
        );
    }


    // private updateUser() {
    //     let user: User = this.frmUser.value;

    //     // let reportsTo: Array<{ reportToId: number }> = []
    //     // const reportsToList = CommonUtility.getSelectedCheckboxList(this.frmUser.value.reportsTo, this.reportsToData);
    //     // if (CommonUtility.isNotEmpty(reportsToList)) {
    //     //     reportsToList.forEach((item: List) => {
    //     //         reportsTo.push({ reportToId: item.id });
    //     //     });
    //     // }

    //     // let roles: Array<{ id: number }> = [];
    //     // const rolesList = CommonUtility.getSelectedCheckboxList(this.frmUser.value.roles, this.roleData);
    //     // if (CommonUtility.isNotEmpty(rolesList)) {
    //     //     rolesList.forEach((item: List) => {
    //     //         roles.push({ id: item.id });
    //     //     });
    //     // }

    //     this.userData = Object.assign(this.userData, user
    //         //     , {
    //         //     reportsTo: reportsTo,
    //         //     roles: roles
    //         // }
    //     );

    //     this.userService.update(this.userData.id, this.userData)
    //         .subscribe((result: any) => {
    //             if (result.isSuccess) {

    //                 this.notificationService.success("User updated successfully.");
    //                 this.router.navigate(['../..', 'list'], { relativeTo: this.activatedRoute });

    //             }
    //             else {
    //                 this.notificationService.warning(result.message);
    //             }
    //         }, (error) => {
    //             if (error.status === 400 && error.error.modelState) {
    //                 this.error = error.error.modelState[''][0];
    //             } else {
    //                 this.error = 'Something went wrong';
    //             }
    //         });
    // }
    private updateUser() {
        let user: User = this.frmUser.value;

        this.userData = Object.assign(this.userData, user);

        this.userService.update(this.userData.id, this.userData)
            .subscribe(
                (result: any) => {
                    this.cancel();
                    // console.log('Update Response:', result);
                    if (result) {

                        // Safely access result.message
                        const errorMessage = result?.message || 'An unexpected error occurred.';
                        this.notificationService.warning(errorMessage);

                    } else {
                        this.notificationService.success("User updated successfully.");
                        this.router.navigate(['../..', 'list'], { relativeTo: this.activatedRoute });
                        this.cancel();

                    }
                },
                (error) => {
                    console.error('Update Error:', error); // Log the error
                    // Handle errors, ensure error.message is safely accessed
                    const errorMessage = error?.error?.modelState?.['']?.[0] || 'Something went wrong';
                    this.error = errorMessage;
                    this.notificationService.error(errorMessage);
                }
            );
    }



    save() {
        this.isFormSubmitted = true;

        if (this.frmUser.invalid) {
            return;
        }

        if (this.isEditMode) {
            this.updateUser(); // Update user
        } else {
            this.createUser(); // Create new user
        }

        // // After creating/updating the user, assign the role
        // const selectedRole = this.frmUser.get('role').value;
        // if (selectedRole) {
        //     this.handleRoleAssignment(selectedRole);
        // }
    }

    private handleRoleAssignment(newRole: string) {
        if (this.isEditMode) {
            this.userService.getRoles(this.userId)
                .subscribe((currentRoles: string[]) => {
                    const oldRole = currentRoles[0]; // Assuming only one role is assigned
                    if (oldRole && oldRole !== newRole) {
                        // Remove the old role
                        this.userService.removeRole(this.userId, oldRole)
                            .subscribe(() => {
                                // Add the new role
                                this.assignRoleToUser(this.userId, newRole);
                            }, (error) => {
                                this.notificationService.error(`Failed to remove old role: ${error.message}`);
                            });
                    } else {
                        // Just add the new role if no old role or same role
                        this.assignRoleToUser(this.userId, newRole);
                    }
                }, (error) => {
                    this.notificationService.error(`Failed to get current roles: ${error.message}`);
                });
        } else {
            // Just assign the new role for new user
            this.assignRoleToUser(this.userId, newRole);
        }
    }

    // Method to assign role to the user
    assignRoleToUser(userId: number, role: string) {
        this.userService.assignRole(userId, role).subscribe(
            (result: any) => {
                this.notificationService.success(`Role '${role}' assigned successfully to user.`);
            },

        );
    }


    setValidators() {
        const rolesList = CommonUtility.getSelectedCheckboxList(this.frmUser.value.roles, this.roleData);

        if (rolesList.some(role => role.name === 'Super Admin')) {
            this.frmUser.controls.parkingAuthorityId.setValidators([]);
        }
        else {
            this.frmUser.controls.parkingAuthorityId.setValidators([Validators.required]);
        }

        this.frmUser.controls.parkingAuthorityId.updateValueAndValidity();
    }

    keyDownHandler(event) {
        if (event.code === 'Space') {
            event.preventDefault();
        }
    }

    cancel() {
       this.router.navigate(['user']);
    }

    ngOnDestroy(): void {
        this.routerSub.unsubscribe();
    }
}