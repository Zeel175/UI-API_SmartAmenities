import { FileUploaderService } from '../core/service';
import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class VendorMaster extends BaseAuditable {
    id: number;
    vendorCode: string;
    vendorName: string;
    email: string;
    phone: string;
    mobile: string;
    baseAddress1: string;
    baseAddress2?: string; // Optional field
    city: string;
    stateId: number;
    countryId: number;
    zipCode: string;
    gstNumber: string;
    termsConditionsId?: number | null;

    vendorContactInformations: VendorContactInformation[] = [];
    vendorAddressInformations: VendorAddressInformation[] = [];
    vendorDocuments: VendorDocuments[] = [];
    vendorProductCategories: VendorProductCategories[] = [];
    name: any;
    code:any;
    vendorTermsDetails: VendorTermsDetail[] = [];
}

export class VendorContactInformation extends BaseAuditable {
    id: number;
    vendorMasterId: number;
    firstName: string;
    lastName: string;
    mobileNo?: string; // Optional field
    landlineNo?: string; // Optional field
    email: string;
    address1: string;
    address2?: string; // Optional field
    city: string;
    stateId: number;
    countryId: number;
    department: string;
    designation: string;
    reportingTo?: string; // Optional field    
}

export class VendorAddressInformation extends BaseAuditable {
    id: number;
    vendorMasterId: number;
    location: string;
    mobileNo: string;
    landlineNo: string;
    email: string;
    address1: string;
    address2?: string; // Optional field
    city: string;
    stateId: number;
    countryId: number;
    gstNo: string;
}

export class VendorDocuments extends BaseAuditable {
    id?: number;
    vendorMasterId?: number;
    documentName: string;
    pictureId?: number;
    picture: DocumentDetails;
    uploader: FileUploaderService;
    hideUploader: boolean;
    uniqueId: string
}

export class VendorProductCategories extends BaseAuditable {
    id: number;
    vendorMasterId: number;
    productCategoryId: number;
    expectedDeliveryDays: number;
}

export class VendorTermsDetail extends BaseAuditable{
  id: number;
  termsConditionId: number;
  productCategoryId: number;
  isDeleted: boolean;
    termsCondition: string;
}