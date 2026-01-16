import { BaseAuditable } from './base-auditable';

export class MaterialRequisition extends BaseAuditable {
    id: number;
    materialRequisitionCode: string;
    materialRequisitionDate: Date;
    warehouseId?: number;
    warehouseName: string;
    expectedDate: Date;
    customerId?: number;
    customerName: string;
    customerPO: string;
    date: Date;
    shippingAddressId?: number;
    shippingAddress: string;
    billingAddressId?: number;
    billingAddress: string;
    statusId?: number;
    isReject: boolean;
    isApprove: boolean;
    status: string;
    remarks: string;

    materialRequisitionProductDetails: MaterialRequisitionProductDetails[];
    name: any;
    description: any;
}

export class MaterialRequisitionProductDetails extends BaseAuditable {
    id: number;
    materialRequisitionId: number;
    productId: number;
    productName: string;
    productDescription: string;
    quantity: number;
    remainingQuantity?: number | null; 
    uomId: number;
    uomName: string;
    productCustomerPO?:string;
    productDate?: Date;
    vendorId?: number;
    vendorName: string;
    materialRawProductDetails: MaterialRawProductDetail[];
    qty: number;
    remainingQty: number;
}

export class MaterialRawProductDetail extends BaseAuditable {
    id: number;
    inputProductCode: number | null;  
    inputProductName?: string | null;
    qty: number | null; 
    remainingQty?: number | null; 
    uom: string | null;  
    drawingNumber: string | null; 
    materialProductRequisitionId: number | null;
    rawProductCustomerPO?: string;
    rawProductDate?: Date;
    purchaseOrderCode?: string;
    purchaseOrderDate?: Date;
    vendorId?: number;
}