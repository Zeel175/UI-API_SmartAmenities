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
import { GuestFamilyMember, GuestMaster } from 'app/model';
import { UnitService } from 'app/modules/unit/unit.service';
import { ToastrService } from 'ngx-toastr';
import { GuestMasterService } from '../guest-master.service';

@Component({
    selector: 'guest-master-add-edit',
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
export class GuestMasterAddEditComponent implements OnInit {
    guestId: number;
    isEditMode = false;
    frmGuest: FormGroup;
    units: any[] = [];
    page = ApplicationPage.guestMaster;
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
        private guestService: GuestMasterService,
        private unitService: UnitService,
        private notificationService: ToastrService,
        private permissionService: PermissionService
    ) {
        this.frmGuest = this.createForm();
    }

    ngOnInit(): void {
        this.IsViewPermission = this.permissionService.hasPermission('Guest (PER_GUEST) - View');
        this.loadUnits();
        this.resolveRoute();
    }

    get familyMembers(): FormArray {
        return this.frmGuest.get('familyMembers') as FormArray;
    }

    private createForm(): FormGroup {
  return this.fb.group({
    code: [{ value: '', disabled: true }], // server generates
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', Validators.email],
    mobile: [''],
    unitId: [null, Validators.required],
    cardId: [{ value: '', disabled: true }],   // ✅ add
    qrId: [{ value: '', disabled: true }],     // ✅ add (if backend supports this field)
    isActive: [true],

    // backend will set these; keep null or remove from form
    fromDateTime: [null],
    toDateTime: [null]
  });
}



    private createFamilyMemberGroup(member?: GuestFamilyMember): FormGroup {
        return this.fb.group({
            id: [member?.id || 0],
            guestMasterId: [member?.guestMasterId || 0],
            unitIds: [member?.unitIds || []],
            firstName: [member?.firstName || '', Validators.required],
            lastName: [member?.lastName || '', Validators.required],
            email: [member?.email || '', Validators.email],
            mobile: [member?.mobile || ''],
            faceId: [member?.faceId || ''],
            fingerId: [member?.fingerId || ''],
            cardId: [member?.cardId || ''],
            qrId: [member?.qrId || ''],
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
                this.guestId = +params['id'];
                this.getGuestDetails();
            }
        });
    }

    private loadUnits(): void {
        this.unitService.getUnits(1, 1000).subscribe((result: any) => {
            this.units = result.items || result || [];
        });
    }

    private getGuestDetails(): void {
        this.guestService.getGuestById(this.guestId).subscribe(
            (result: any) => {
                const data: GuestMaster = result;
                this.setGuestData(data);
                //this.loadGuestDocuments();
            },
            () => this.notificationService.error('Failed to load guest details.')
        );
    }

    private setGuestData(guest: GuestMaster): void {
        this.frmGuest.patchValue({
            code: guest.code,
            firstName: guest.firstName,
            lastName: guest.lastName,
            email: guest.email,
            mobile: guest.mobile,
            cardId: guest.cardId,
            qrId: guest.qrId,
            unitId: guest.unitId ?? null,
            isActive: guest.isActive,
            isResident: guest.isResident ?? false
        });

        if (this.frmGuest.get('familyMembers')) {
            this.familyMembers.clear();
            (guest.familyMembers || []).forEach(member => this.familyMembers.push(this.createFamilyMemberGroup(member)));
        }
        // 🔹 Set profile photo preview (edit mode)
        if (guest.profilePhoto && guest.profilePhoto.trim() !== '') {
            if (guest.profilePhoto) {
                this.profilePhotoPreview = this.getBackendUrl(guest.profilePhoto);
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

    private loadGuestDocuments(): void {
        if (!this.guestId) return;

        this.guestService.getGuestDocuments(this.guestId).subscribe({
            next: (docs) => {
                this.existingDocuments = docs || [];
            },
            error: () => {
                this.notificationService.error('Failed to load documents');
            }
        });
    }
    deleteExistingDocument(documentId: number): void {
        this.guestService.deleteGuestDocument(documentId).subscribe({
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

        if (this.frmGuest.invalid) {
            this.frmGuest.markAllAsTouched();
            return;
        }

        const v = this.frmGuest.getRawValue();

        if (this.isEditMode) {
            const payload = {
                id: this.guestId,
                firstName: v.firstName,
                lastName: v.lastName,
                email: v.email,
                mobile: v.mobile,
                unitId: v.unitId,
                cardId: v.cardId,
                qrId: v.qrId,
                isActive: v.isActive
            };

            this.guestService.updateGuest(payload).subscribe({
                next: () => {
                    this.notificationService.success('Guest updated successfully.');
                    this.router.navigate(['/guest-master']);
                },
                error: () => this.notificationService.warning('Failed to update guest.')
            });
            return;
        }

        const payload = [
            {
                firstName: v.firstName,
                lastName: v.lastName,
                email: v.email,
                mobile: v.mobile,
                unitId: v.unitId,
                cardId: v.cardId,
                qrId: v.qrId,
                isActive: v.isActive
            }
        ];

        this.guestService.createGuest(payload).subscribe({
            next: () => {
                this.notificationService.success('Guest created successfully.');
                this.router.navigate(['/guest-master']);
            },
            error: () => this.notificationService.warning('Failed to save guest.')
        });
    }



    cancel(): void {
        this.router.navigate(['guest-master']);
    }
}
