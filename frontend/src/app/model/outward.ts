import { BaseAuditable } from './base-auditable';
import { DocumentDetails } from './document-details';

export class Outward extends BaseAuditable {
    id?: number;
    outwardCode?: string;
    outwardDate?: Date;
    fromWarehouseId?: number;
    fromWarehouseName?: string;
    inventoryTypeId?: number;
    inventoryTypeName?: string;
    customerId?: number;
    customerName?: string;
    vendorId?: number;
    vendorName?: string;
    warehouseId?: number;
    warehouseName?: string;
    materialRequisitionId?: number;
    materialRequisitionCode?: string;
    address?: string;
    billingAddress?: string;
    supportingDocumentNo?: string|null;
    supportingDocumentDate?: Date|null;
    supportingDocumentId?: bigint|null;
    supportingDocument?: DocumentDetails;
    remarks?: string|null;

    outwardProductDetails: OutwardProductDetails[];
}

export class OutwardProductDetails extends BaseAuditable {
    id?: number;
    outwardId?: number;
    productId?: number;
    productName?: string;
    productDescription?: string;
    quantity?: number;
    uomId?: number;
    uomName?: string;
    remarks?: string;
    mrQuantity?: number;
    pastIssuedQuantity?: number;
    remainingQuantity?: number;
    isRejected?: boolean;
    rejectedDate?: Date;
    stockQuantity?: number;
}