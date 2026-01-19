import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, PermissionType } from 'app/core';
import { PermissionService } from 'app/core/service/permission.service';
import { ResidentFamilyMember, ResidentMaster } from 'app/model';
import { UnitService } from 'app/modules/unit/unit.service';
import { ToastrService } from 'ngx-toastr';
import { ResidentMasterService } from '../resident-master.service';

@Component({
    selector: 'resident-master-add-edit',
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
        MatButtonModule,
        MatCheckboxModule,
        MatIconModule,
        CommonModule,
    ]
})
export class ResidentMasterAddEditComponent implements OnInit {
    residentId: number;
    isEditMode = false;
    frmResident: FormGroup;
    units: any[] = [];
    page = ApplicationPage.residentMaster;
    permissions = PermissionType;
    isFormSubmitted = false;
    IsViewPermission = false;
    profilePhotoFile: File | null = null;
    profilePhotoPreview: string | null = null;
    selectedDocuments: File[] = [];
    existingDocuments: any[] = [];

    constructor(
        private activatedRoute: ActivatedRoute,
        private router: Router,
        private fb: FormBuilder,
        private residentService: ResidentMasterService,
        private unitService: UnitService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) {
        this.frmResident = this.createForm();
    }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Resident (PER_RESIDENT) - View');
        this.loadUnits();
        this.resolveRoute();
    }

    get familyMembers(): FormArray {
        return this.frmResident.get('familyMembers') as FormArray;
    }

    private createForm(): FormGroup {
        return this.fb.group({
            code: [{ value: '', disabled: true }],
            parentFirstName: ['', Validators.required],
            parentLastName: ['', Validators.required],
            email: ['', Validators.email],
            mobile: [''],
            faceId: [{ value: '', disabled: true }],
            fingerId: [{ value: '', disabled: true }],
            cardId: [{ value: '', disabled: true }],
            qrId: [{ value: '', disabled: true }],
            unitIds: [[]],
            isActive: [true],
            isResident: [false],
            familyMembers: this.fb.array([])
        });
    }

    private createFamilyMemberGroup(member?: ResidentFamilyMember): FormGroup {
        return this.fb.group({
            id: [member?.id || 0],
            code: [{ value: member?.code || '', disabled: true }],
            residentMasterId: [member?.residentMasterId || 0],
            unitIds: [member?.unitIds || []],
            firstName: [member?.firstName || '', Validators.required],
            lastName: [member?.lastName || '', Validators.required],
            email: [member?.email || '', Validators.email],
            mobile: [member?.mobile || ''],
            faceId: [{ value: member?.faceId || '', disabled: true }],
            fingerId: [{ value: member?.fingerId || '', disabled: true }],
            cardId: [{ value: member?.cardId || '', disabled: true }],
            qrId: [{ value: member?.qrId || '', disabled: true }],
            isActive: [member?.isActive ?? true],
            isResident: [member?.isResident ?? false],
            profilePhoto: [member?.profilePhoto || null],
            profilePhotoFile: [null]
        });
    }

    addFamilyMember(): void {
        this.familyMembers.push(this.createFamilyMemberGroup());
    }

    removeFamilyMember(index: number): void {
        if (this.familyMembers.length > 0) {
            this.familyMembers.removeAt(index);
        }
    }

    private resolveRoute(): void {
        this.activatedRoute.params.subscribe((params) => {
            this.isEditMode = !!params['id'];
            if (this.isEditMode) {
                this.residentId = +params['id'];
                this.getResidentDetails();
            }
        });
    }

    private loadUnits(): void {
        this.unitService.getUnits(1, 1000).subscribe((result: any) => {
            this.units = result.items || result || [];
        });
    }

    private getResidentDetails(): void {
        this.residentService.getResidentById(this.residentId).subscribe(
            (result: any) => {
                const data: ResidentMaster = result;
                this.setResidentData(data);
                this.loadResidentDocuments();
            },
            () => this.notificationService.error('Failed to load resident details.')
        );
    }

    private setResidentData(resident: ResidentMaster): void {
        this.frmResident.patchValue({
            code: resident.code,
            parentFirstName: resident.parentFirstName,
            parentLastName: resident.parentLastName,
            email: resident.email,
            mobile: resident.mobile,
            faceId: resident.faceId,
            fingerId: resident.fingerId,
            cardId: resident.cardId,
            qrId: resident.qrId,
            unitIds: resident.unitIds || [],
            isActive: resident.isActive,
            isResident: resident.isResident ?? false
        });

        this.familyMembers.clear();
        (resident.familyMembers || []).forEach(member => this.familyMembers.push(this.createFamilyMemberGroup(member)));
        // 🔹 Set profile photo preview (edit mode)
        if (resident.profilePhoto && resident.profilePhoto.trim() !== '') {
            if (resident.profilePhoto) {
  this.profilePhotoPreview = this.getBackendUrl(resident.profilePhoto);
} else {
  this.profilePhotoPreview = null;
}

        } else {
            this.profilePhotoPreview = null;
        }


    }
    private readonly BACKEND_BASE_URL = 'https://localhost:7032';

getBackendUrl(path: string | null | undefined): string {
  if (!path) return '';

  if (path.startsWith('http')) {
    return path;
  }

  return `https://localhost:7032${path}`;
}


    onProfilePhotoSelected(event: Event): void {
        const input = event.target as HTMLInputElement;

        if (input.files && input.files.length > 0) {
            this.profilePhotoFile = input.files[0];

            const reader = new FileReader();
            reader.onload = () => {
                this.profilePhotoPreview = reader.result as string;
            };
            reader.readAsDataURL(this.profilePhotoFile);
        }
    }
    onDocumentsSelected(event: Event): void {
        const input = event.target as HTMLInputElement;

        if (input.files && input.files.length > 0) {
            Array.from(input.files).forEach(file => {
                this.selectedDocuments.push(file);
            });
        }

        // reset input so same file can be reselected
        input.value = '';
    }

    removeSelectedDocument(index: number): void {
        this.selectedDocuments.splice(index, 1);
    }

    private loadResidentDocuments(): void {
        if (!this.residentId) return;

        this.residentService.getResidentDocuments(this.residentId).subscribe({
            next: (docs) => {
                this.existingDocuments = docs || [];
            },
            error: () => {
                this.notificationService.error('Failed to load documents');
            }
        });
    }
    deleteExistingDocument(documentId: number): void {
    this.residentService.deleteResidentDocument(documentId).subscribe({
        next: () => {
            this.existingDocuments =
                this.existingDocuments.filter(d => d.id !== documentId);
            this.notificationService.success('Document deleted');
        },
        error: () => {
            this.notificationService.error('Failed to delete document');
        }
    });
}
getDocumentUrl(path: string | null | undefined): string {
  return this.getBackendUrl(path);
}


openSelectedDocument(file: File): void {
    const fileUrl = URL.createObjectURL(file);
    window.open(fileUrl, '_blank');

    // Optional cleanup after some time
    setTimeout(() => {
        URL.revokeObjectURL(fileUrl);
    }, 1000);
}
getFamilyPhotoPreview(memberGroup: FormGroup): string | null {
  const value = memberGroup.get('profilePhoto')?.value;
  if (!value) return null;

  // Base64 (new upload)
  if (value.startsWith('data:image')) {
    return value;
  }

  // Existing backend path
  return this.getBackendUrl(value);
}

onFamilyPhotoSelected(event: Event, index: number): void {
  const input = event.target as HTMLInputElement;

  if (!input.files || input.files.length === 0) return;

  const file = input.files[0];

  const memberGroup = this.familyMembers.at(index) as FormGroup;
  memberGroup.patchValue({
    profilePhotoFile: file
  });

  const reader = new FileReader();
  reader.onload = () => {
    memberGroup.patchValue({
      profilePhoto: reader.result as string
    });
  };
  reader.readAsDataURL(file);
}



    save(): void {
        this.isFormSubmitted = true;
        if (this.frmResident.invalid) {
            this.frmResident.markAllAsTouched();
            return;
        }

        const formValue = this.frmResident.getRawValue();
        const familyMembers = (formValue.familyMembers || []).map((member: any) => ({
            ...member,
            unitIds: member.unitIds || [],
            residentMasterId: this.isEditMode ? this.residentId : 0,
            id: member.id || 0
        }));

        const formData = new FormData();

        // Basic fields
        formData.append('id', this.isEditMode ? this.residentId.toString() : '0');
        formData.append('code', formValue.code || '');
        formData.append('parentFirstName', formValue.parentFirstName);
        formData.append('parentLastName', formValue.parentLastName);
        formData.append('email', formValue.email || '');
        formData.append('mobile', formValue.mobile || '');
        formData.append('faceId', formValue.faceId || '');
        formData.append('fingerId', formValue.fingerId || '');
        formData.append('cardId', formValue.cardId || '');
        formData.append('qrId', formValue.qrId || '');
        formData.append('isActive', formValue.isActive.toString());
        formData.append('isResident', formValue.isResident.toString());

        // Units
        (formValue.unitIds || []).forEach((id: number, index: number) => {
            formData.append(`unitIds[${index}]`, id.toString());
        });

        // Family Members
        familyMembers.forEach((member: any, i: number) => {

    Object.keys(member).forEach(key => {
        if (key === 'profilePhotoFile' || key === 'profilePhoto') {
            return;
        }

        if (Array.isArray(member[key])) {
            member[key].forEach((val: any, j: number) => {
                formData.append(`familyMembers[${i}].${key}[${j}]`, val);
            });
        } else {
            formData.append(`familyMembers[${i}].${key}`, member[key] ?? '');
        }
    });

    // 🔹 FILE GOES SEPARATELY
    if (member.profilePhotoFile) {
        formData.append(
            `familyMembers[${i}].ProfilePhotoFile`,
            member.profilePhotoFile
        );
    }
});


        // Profile Photo
        if (this.profilePhotoFile) {
            formData.append('ProfilePhotoFile', this.profilePhotoFile);
        }
        // Resident Documents (OPTIONAL)
        this.selectedDocuments.forEach(file => {
            formData.append('Documents', file);
        });

        const request$ = this.isEditMode
            ? this.residentService.residentUpdate(formData)
            : this.residentService.addResident(formData);

        request$.subscribe(
            () => {
                this.notificationService.success(
                    `Resident ${this.isEditMode ? 'updated' : 'created'} successfully.`
                );
                this.router.navigate(['/resident-master']);
            },
            () => this.notificationService.warning('Failed to save resident.')
        );

    }

    cancel(): void {
        this.router.navigate(['resident-master']);
    }
}
