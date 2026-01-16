// app/model/sales-order.model.ts

import { FileUploaderService } from 'app/core';
import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class SalesOrder extends BaseAuditable {
    id: number;
    salesOrderCode: string;
    revisionNo: string;
    salesPersonId: number;
    salesPersonName?: string; // <-- UI uses this!
    customerId: number;
    customerDetailId: number;
    customerName?: string;
    taxRegistrationNumber?: string;
    companyId?: number;
    opportunityId?: number;
    customerPONumber: string;
    date: Date;
    salesOrderTypeId: number;
    currencyId: number;
    termsCondition?: string;
    totalProductAmount?: number;
    discountTypeId?: number;
    discountTypeAmount?: number;
    discountAmount?: number;
    subTotal?: number;
    totalTaxAmount?: number;
    totalAmount?: number;
    isReject: boolean;
    isApprove: boolean;
    isActive: boolean; // <-- important for row styling
    status: string;

    // Nested arrays for products and attachments
    salesOrderProductDetails: SalesOrderProductDetails[] = [];
    salesOrderAttachments?: SalesOrderAttachments[] = [];
    salesOrderTermsDetails: SalesOrderTermsDetails[] = [];

    materialRequisitionId?: number;
    outwardData?: any[];

    // UI-only fields:
    approvalStatus?: string;
    
}

export class SalesOrderProductDetails extends BaseAuditable {
    id: number;
    salesOrderId?: number; // optional for insert
    productId: number;
    productName?: string; // often needed for display/search
    qty: number;
    uomId: number;
    uomName?: string;
    amount: number;
    discountTypeId?: number;
    discountTypeAmount?: number;
    discountAmount?: number;
    subTotal?: number;
    taxTypeAmount?: number;
    remarks?: string;
    expectedDeliveryDate?: Date;
    typeOfConnection?: string;

    // Status fields (added for UI logic!)
    productionWorkOrderStatus?: string;
    productionPlanningStatus?: string;
    materialRequisitionStatus?: string;
    purchaseOrderStatus?: string;
    inwardStatus?: string;
    outwardStatus?: string;

}

export class SalesOrderAttachments extends BaseAuditable {
    id?: number;
    salesOrderId?: number;
    documentName: string;
    pictureId?: number;
    picture?: DocumentDetails;
    hideUploader?: boolean;
    uniqueId?: string;
    uploader: FileUploaderService;
}

export class SalesOrderTermsDetails extends BaseAuditable {
    id: number;
    salesOrderId: number;
    termsConditionId?: number;       // Optional (for backward compatibility)
    termsConditionIds?: number[];    // <-- Add this line for multi-select!
    termsCondition: string;
}

