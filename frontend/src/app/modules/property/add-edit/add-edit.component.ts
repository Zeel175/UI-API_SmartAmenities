import { CommonModule } from '@angular/common';
import { Component, TemplateRef, ViewChild } from '@angular/core';
import { FormsModule, ReactiveFormsModule, UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ApplicationPage, CommonUtility, FileUploaderService, PermissionType } from 'app/core';
import {  FileConfiguration } from 'app/model';
import { NgxMaskDirective, provideNgxMask } from 'ngx-mask';
import { Subscription } from 'rxjs';
import { PropertyService } from '../property.service';
import { ToastrService } from 'ngx-toastr';
import { PermissionService } from 'app/core/service/permission.service';
import { SharedStateService } from 'app/shared/services/shared-state.service';
import { Property } from 'app/model/property';

@Component({
    selector: 'property-add-edit',
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
        MatButtonModule, CommonModule, //MatValidationMessage,

    ],
    providers: [
        provideNgxMask()
    ]
})
export class PropertyAddEditComponent {
propertyData: Property;
    propertyId: number;
    isEditMode: boolean;
    frmProperty: UntypedFormGroup;
    routerSub: Subscription;
    isFormSubmitted: boolean;
    error: string;
    page: string = ApplicationPage.property;
    permissions = PermissionType;
    IsViewPermission: boolean = false;
    propertyLogoDocument: any = null; // This will hold the document details
    imagePreview: string | null = null;
    columns: any[] = [];
    @ViewChild('createdDateTemplate') createdDateTemplate!: TemplateRef<any>;

    fileOptions: FileConfiguration = {
        maxAllowedFile: 1,
        completeCallback: this.uploadCompleted.bind(this),
        onWhenAddingFileFailed: this.uploadFailed.bind(this)
    };

    fileUploader: FileUploaderService = new FileUploaderService(this.fileOptions);

    constructor(private activatedRoute: ActivatedRoute, private router: Router,
        private formBuilder: UntypedFormBuilder, private propertyService: PropertyService,
        private notificationService: ToastrService, private permissionService: PermissionService,
        private sharedStateService: SharedStateService) {
        this.createForm();
    }

    ngOnInit(): void {
        this.getPropertyRoute();
        this.IsViewPermission = this.permissionService.hasPermission('Property (PER_PROPERTY) - View');
        this.sharedStateService.columns$.subscribe(columns => {
            if (columns && columns.length) {
                this.columns = columns;
            } else {
                this.columns = this.getDefaultColumns(); // Fallback to default if no saved state
            }
        });
    }

    getDefaultColumns(): any[] {
        // Default column setup if no state exists
        return [
            { name: 'Property Name', prop: 'propertyName', visible: true },
            { name: 'Contact No', prop: 'contactNo', visible: true },
            { name: 'Alias', prop: 'alias', visible: true },
            { name: 'Address1', prop: 'address1', visible: true },
            { name: 'Address2', prop: 'address2', visible: true },
            { name: 'Address3', prop: 'address3', visible: true },
            { name: 'Country', prop: 'country', visible: true },
            { name: 'State', prop: 'state', visible: true },
            { name: 'City', prop: 'city', visible: true },
            { name: 'PinCode', prop: 'pincode', visible: true },
            { name: 'Email', prop: 'email', visible: true },
            { name: 'WebSite', prop: 'website', visible: true },
            { name: 'Phone', prop: 'phone', visible: true },
            { name: 'GST No', prop: 'gstNo', visible: true },
            { name: 'Latitude', prop: 'latitude', visible: true },
            { name: 'Longitude', prop: 'longitude', visible: true },
            { name: 'Pan No', prop: 'panNo', visible: true },
            { name: 'Msme No', prop: 'msmeNo', visible: true, cellTemplate: this.createdDateTemplate }
        // other columns
      ];
      }

      onColumnStateChanged(event) {
        const columnState = event.columnApi.getColumnState();
        this.sharedStateService.setColumns(columnState); // Store column state in service
    }

    private getPropertyRoute() {
        this.routerSub = this.activatedRoute.params.subscribe((params) => {
            this.isEditMode = !CommonUtility.isEmpty(params["id"]);
            this.createForm();
            if (this.isEditMode) {
                this.propertyId = +params["id"];
                this.getPropertyDetails();
            }
            else {
                this.propertyData = new Property();
            }
        });
    }

    private getPropertyDetails() {
        // this.companyService.getById(this.companyId)
        //     .subscribe((result: Company) => {
        //         this.companyData = result;
        //         this.setCompanyData();
        //     }, (error) => {
        //         console.log(error);
        //     });


        this.propertyService.getPropertyById(this.propertyId)
            .subscribe((result: any) => {
                this.propertyData = result;
                this.setPropertyData();
            }, (error) => {
                console.log(error);
            });
    }

     private setPropertyData() {
        //this.frmProperty.patchValue(this.propertyData);
        this.frmProperty.patchValue({
            propertyName: this.propertyData.propertyName,
            contactNo: this.propertyData.contactNo,
            alias: this.propertyData.alias,
            address1: this.propertyData.address1,
            address2: this.propertyData.address2,
            address3: this.propertyData.address3,
            country: this.propertyData.country,
            state: this.propertyData.state,
            city: this.propertyData.city,
            pincode: this.propertyData.pincode,
            email: this.propertyData.email,
            website: this.propertyData.website,
            phone: this.propertyData.phone,
            gstNo: this.propertyData.gstNo,
            panNo: this.propertyData.panNo,
            latitude: this.propertyData.latitude,
            longitude: this.propertyData.longitude,
            msmeNo: this.propertyData.msmeNo,
            propertyLogo: this.propertyData.propertyLogo // Add this line
        });
        // Set image preview if property logo exists
        if (this.propertyData?.propertyLogo) {
            this.imagePreview = this.propertyData.propertyLogo;
        }
    }

    createForm() {
        this.frmProperty = this.formBuilder.group({
            // Assuming these are the fields required for a property entity.
            // Include validators as per your validation rules.
            propertyName: ['', [Validators.required, Validators.maxLength(100)]],
            contactNo: ['', [Validators.required, Validators.maxLength(10)]],
            alias: [''],
            address1: ['', [Validators.required]],
            address2: [''],
            address3: [''],
            country: ['', [Validators.required, Validators.maxLength(50)]],
            state: ['', [Validators.required, Validators.maxLength(50)]],
            city: ['', [Validators.required, Validators.maxLength(50)]],
            pincode: ['', [Validators.required, Validators.maxLength(10)]],
            email: ['', [Validators.email, Validators.maxLength(50)]],
            website: ['', Validators.required],
            phone: ['', [Validators.maxLength(15)]],
            gstNo: ['', Validators.required],
            panNo: ['', Validators.required],
            latitude: [''],
            longitude: [''],
            msmeNo: [''],
            propertyLogo: [''] // Add this line
            //isActive: [false],

            //currency: ['']
            // Add more form controls here if needed

        });
    }

    save() {
        this.isFormSubmitted = true;

        if (this.frmProperty.invalid) {
            return;
        }

        if (this.isEditMode) {
            this.updateProperty();
        } else {
            this.createProperty();
        }
    }

    removeFile(event) {
        // Clear both preview and form value
        this.imagePreview = null;
        if (this.propertyData) {
            this.propertyData.propertyLogo = null;
        }
        if (this.frmProperty.contains('propertyLogo')) {
            this.frmProperty.patchValue({
                propertyLogo: null
            });
        }
        // Reset file input if it exists
        const fileInput = document.querySelector('input[type="file"]') as HTMLInputElement;
        if (fileInput) {
            fileInput.value = '';
        }
    }

    canHideUploader(): boolean {
        let hideUploader: boolean = false;

        if ((this.propertyData && this.propertyData.propertyLogoId > 0) || this.fileUploader.hasFile()) {
            hideUploader = true;
        }

        return hideUploader;
    }

    private createProperty() {
        let property: Property = Object.assign({}, this.frmProperty.value);

        if (this.propertyData?.propertyLogo) {
            property.propertyLogo = this.propertyData.propertyLogo;
        }
        this.propertyService.addProperty(property)
            .subscribe({
                next: (result: any) => {
                    if (result && !isNaN(result)) {
                        this.notificationService.success("Property saved successfully.");
                        this.cancel(); // This will navigate to list page
                    } else {
                        // Check if there's a message in the result
                        if (result && result.message) {
                            this.notificationService.warning(result.message);
                        } else {
                            this.notificationService.warning("Failed to save property.");
                        }
                    }
                },
                error: (error) => {
                    if (error.status === 400 && error.error.modelState) {
                        this.error = error.error.modelState[''][0];
                        this.notificationService.error(this.error);
                    } else {
                        this.error = 'Something went wrong';
                        this.notificationService.error(this.error);
                    }
                }
            });
    }

    private updateProperty() {
        let property: Property = Object.assign(this.propertyData, this.frmProperty.value);

        if (this.frmProperty.get('propertyLogo')?.value) {
            property.propertyLogo = this.frmProperty.get('propertyLogo').value;
        }

        this.propertyService.updateProperty(property)
            .subscribe({
                next: (result: any) => {
                    if (result && !isNaN(result)) {
                        this.notificationService.success("Property Updated successfully.");
                        this.cancel(); // This will navigate to list page
                    } else {
                        // Check if there's a message in the result
                        if (result && result.message) {
                            this.notificationService.warning(result.message);
                        } else {
                            this.notificationService.warning("Failed to update property.");
                        }
                    }
                },
                error: (error) => {
                    if (error.status === 400 && error.error.modelState) {
                        this.error = error.error.modelState[''][0];
                        this.notificationService.error(this.error);
                    } else {
                        this.error = 'Something went wrong';
                        this.notificationService.error(this.error);
                    }
                }
            });
    }

    // private uploadDocuments(id: number) {
    //     this.fileUploader.uploadFiles({
    //         uploadType: UploadType.Property,
    //         id: id.toString()
    //     });
    // }

    private uploadCompleted() {
        this.cancel();

        if (this.isEditMode) {
            this.notificationService.success("Company updated successfully.");
        } else {
            this.notificationService.success("Company saved successfully.");
        }
    }

    private uploadFailed() {
        this.notificationService.warning("You are only allowed to upload jpg/jpeg/png/pdf/doc files.");
    }

    onSelectFile(event) {
        const file = event.target.files[0];

        // Reset file input if image already exists
        if ((this.imagePreview || this.propertyData?.propertyLogo) && !event.target.value.match(/fakepath/)) {
            this.notificationService.warning('Please remove existing image first before uploading a new one.');
            event.target.value = ''; // Reset file input
            return;
        }

        if (file) {
            // Validate file type
            const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png'];
            if (!allowedTypes.includes(file.type)) {
                this.notificationService.warning('Please select only jpg, jpeg or png files.');
                event.target.value = ''; // Reset file input
                return;
            }

            // Validate file size (e.g., 2MB limit)
            const maxSize = 2 * 1024 * 1024; // 2MB in bytes
            if (file.size > maxSize) {
                this.notificationService.warning('File size should not exceed 2MB.');
                event.target.value = ''; // Reset file input
                return;
            }

            // Convert to base64
            const reader = new FileReader();
            reader.onload = () => {
                const base64String = reader.result as string;
                // Set both preview and form value
                this.imagePreview = base64String;
                if (this.propertyData) {
                    this.propertyData.propertyLogo = base64String;
                }

                // Update form
                if (!this.frmProperty.contains('propertyLogo')) {
                    this.frmProperty.addControl('propertyLogo', this.formBuilder.control(''));
                }
                this.frmProperty.patchValue({
                    propertyLogo: base64String
                });
            };
            reader.readAsDataURL(file);
        }
    }

    cancel() {
       this.router.navigate(['property']);
    }

    ngOnDestroy(): void {
        this.routerSub.unsubscribe();
    }
}
