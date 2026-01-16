import { BaseAuditable } from "./base-auditable";

export class PurchaseInquiry extends BaseAuditable {
    id: number;
    purchaseInquiryCode: string; // Auto-generated code
    inquiryDate: Date; // Manual entry
    inquiryDescription: string; // Optional field
    inquiryStartDate: Date; // Manual entry
    inquiryEndDate: Date; // Manual entry
    remarks: string; // Optional field

    purchaseInquiryDetails: PurchaseInquiryDetail[];
    vendorDetails: VendorDetail[];

}

export class PurchaseInquiryDetail extends BaseAuditable {
    id: number;
    purchaseInquiryId: number;
    productCode: number; // Foreign key to Product master
    qty: number; // Integer quantity
    expectedDeliveryDate: Date; // Expected delivery date
    remarks: string; // Optional remarks
}
// Class to represent Vendor (if you need to include detailed vendor information)
export class VendorDetail extends BaseAuditable {
    id: number;
    vendorCode: number; // Foreign key, if you need to link to the vendor
    purchaseInquiryId: number;
    remarks: string; // Optional remarks
}