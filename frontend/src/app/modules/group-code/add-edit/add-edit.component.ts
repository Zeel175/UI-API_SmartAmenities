import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCard, MatCardContent, MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { FuseCardComponent } from '@fuse/components/card';
import { ApplicationPage, CommonUtility, ListService, PermissionType } from 'app/core';
import { GroupCode } from 'app/model';
import { Subscription } from 'rxjs';
import { GroupCodeService } from '../group-code.service';
import { ToastrModule, ToastrService } from 'ngx-toastr';
import { PermissionService } from 'app/core/service/permission.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatValidationMessage } from 'app/shared';

@Component({
    selector: 'group-code-add-edit',
    templateUrl: './add-edit.component.html',
    standalone: true,
    imports: [
        CommonModule,
        RouterModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatIconModule,
        MatProgressSpinnerModule,
        MatTooltipModule,
        ToastrModule,
        MatValidationMessage,
        MatCardContent,
        MatCard

    ],
    providers: [GroupCodeService, ListService, ToastrService]
})
export class GroupCodeAddEditComponent implements OnInit, OnDestroy {
    groupCodeData: GroupCode;
    groupCodeId: number;
    isEditMode: boolean = false;
    frmGroupCode: UntypedFormGroup;
    routerSub: Subscription;
    isFormSubmitted: boolean;
    page: string = ApplicationPage.group_code;
    permissions = PermissionType;

    error: string;
    isSaving: boolean = false;
    returnUrl: string | null = null;
    routeQuerySub: Subscription;
    isAddressMissing: boolean = false;
    IsViewPermission: boolean = false;
    IsEditPermission: boolean = false;
    IsCreatePermission: boolean = false;

    constructor(private activatedRoute: ActivatedRoute, private router: Router,
        private formBuilder: UntypedFormBuilder, private groupCodeService: GroupCodeService,
        private notificationService: ToastrService, private listService: ListService,
        private permissionService: PermissionService) {
        this.createForm();
    }

    get CanSave(): boolean {
        return this.isEditMode ? this.IsEditPermission : this.IsCreatePermission;
    }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - View');
        this.IsEditPermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - Edit');
        this.IsCreatePermission = this.permissionService.hasPermission('GroupCode (PER_GROUP_CODE) - Add');

        this.getGroupCodeRoute();
        // if (!this.isEditMode) {
        //     this.generateCode();
        // }
    }
    private getGroupCodeRoute() {
    this.routerSub = this.activatedRoute.params.subscribe((params) => {
      this.isEditMode = !CommonUtility.isEmpty(params['id']);
      this.createForm(); // recreate form with/without code control based on mode
      if (this.isEditMode) {
        this.groupCodeId = +params['id'];
        this.getGroupCodeDetails();
      }
    });
  }

    private getGroupCodeDetails() {
  this.groupCodeService.getByIdAsync(this.groupCodeId)
    .subscribe(
      (result: GroupCode) => {
        this.groupCodeData = result;
        this.setGroupCodeData();   // patches form
      },
      (error) => console.error(error)
    );
}

    private setGroupCodeData() {
        this.frmGroupCode.patchValue(this.groupCodeData);
    }

    // private generateCode() {
    //     this.groupCodeService.generateCode()
    //         .subscribe((code: string) => {
    //             this.frmGroupCode.patchValue({ code });
    //         }, (error) => {
    //             console.log(error);
    //         });
    // }

    createForm() {
  const formShape: { [key: string]: any } = {
    // ✅ Always present & disabled in both Add and Edit
    code: [{ value: '', disabled: true }, [Validators.maxLength(50)]],

    name: ['', [Validators.required, Validators.maxLength(100)]],
    groupName: ['', [Validators.required, Validators.maxLength(100)]],
    priority: ['', [Validators.required, Validators.min(1)]],
    value: ['', [Validators.maxLength(255)]],
    isActive: [true],
    // rowVersion: ['AAAAAAc='], // if your API needs it
  };

  this.frmGroupCode = this.formBuilder.group(formShape);
}

    private createGroupCode() {
    const payload: GroupCode = this.frmGroupCode.getRawValue();
    // Note: No 'code' sent in add mode; server will generate it.

    this.groupCodeService.addGroupCode(payload).subscribe(
      () => {
        this.isSaving = false;
        this.cancel();
        this.notificationService.success('Miscellaneous saved successfully.');
      },
      (error) => {
        this.isSaving = false;
        this.error = error;
      }
    );
  }
    private updateGroupCode() {
        let groupCode: GroupCode = this.frmGroupCode.getRawValue();
        //this.groupCodeData = Object.assign(this.groupCodeData, this.groupCodeData, groupCode);
        const payload: GroupCode = {
            ...this.groupCodeData,
            ...groupCode,
            id: this.groupCodeId
        };
        this.groupCodeService.editGroupCode(payload)
            .subscribe(() => {
                this.isSaving = false;
                this.cancel();
                this.notificationService.success("Miscellaneous updated successfully.");
            },
                (error) => {
                    this.isSaving = false;
                    this.error = error;
                });
    }
    // addCustomerDetail(detail?: CustomerDetails) {
    //     this.customerDetails.push(this.formBuilder.group({
    //         location: [detail ? detail.location : '', [Validators.required, Validators.maxLength(256)]],
    //         street: [detail ? detail.street : '', [Validators.maxLength(1028)]],
    //         city: [detail ? detail.city : '', [Validators.required, Validators.maxLength(256)]],
    //         state: [detail ? detail.state : '', [Validators.required, Validators.maxLength(256)]],
    //         country: [detail ? detail.country : '', [Validators.required, Validators.maxLength(256)]],
    //         zipCode: [detail ? detail.zipCode : '', [Validators.maxLength(56)]],
    //         gstinNo: [detail ? detail.gstinNo : '', [Validators.required, Validators.maxLength(256)]],
    //         rowVersion: 'AAAAAAc=',
    //     }));
    //     this.stateOptions.push([]);
    //     this.isAddressMissing = false;
    //     if (detail && detail.country) {
    //         const index = this.customerDetails.length - 1;
    //         this.onCountryChange(detail.country, index);
    //     }
    // }

    save() {
        this.isFormSubmitted = true;
        if (this.frmGroupCode.invalid) {
            return;
        }
        this.isSaving = true;


        if (this.isEditMode) {
            this.updateGroupCode();
        } else {
            this.createGroupCode();
        }
    }

    // cancel() {
    //     if (this.isEditMode) {
    //         this.router.navigate(['../..', 'list'], { relativeTo: this.activatedRoute });
    //     } else {
    //         this.router.navigate(['..', 'list'], { relativeTo: this.activatedRoute });
    //     }
    // }
    // cancel() {
    //     if (this.returnUrl) {
    //         this.router.navigateByUrl(this.returnUrl);
    //     } else if (this.isEditMode) {
    //         this.router.navigate(['../..', 'list'], { relativeTo: this.activatedRoute });
    //     } else {
    //         this.router.navigate(['..', 'list'], { relativeTo: this.activatedRoute });
    //     }
    // }

    cancel() {
        this.router.navigate(['/group-code/list']);
    }

    ngOnDestroy(): void {
        this.routerSub.unsubscribe();
    }
}
