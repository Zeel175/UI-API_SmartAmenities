import { BaseAuditable } from "./base-auditable"; 

// Main Purchase Planning class
export class PurchasePlanning extends BaseAuditable {
    id: number;
    planningCode: string; // Auto-generated code
    planningDate: Date; // System Date Default
    customerCode: number; // Foreign key to Customer Master
    customerPONumber: string; // Optional field
    customerPODate: Date; // Required
    productCode: number; // Foreign key to Product/BOM Master
    qty: number; // Required
    uom: string; // Unit of Measure from BOM Master
    remark: string; // Optional field
    purchasePlanningDetails: PurchasePlanningDetail[]; // Details array
}

// Details related to Purchase Planning
export class PurchasePlanningDetail extends BaseAuditable {
    id: number;
    productCode: number; // Foreign key to Product/BOM Master
    requiredQty: number; // Manual Entry
    vendorCode: number; // Foreign key to Vendor Master
    leadTime: number; // Lead Time (Days) - Manual Entry
    purchaseOrderDate: Date; // Manual Entry
    productionsStartdate: Date; // Manual Entry
    productionsEndDate: Date; // Manual Entry
    purchaseHour: number;
    remark: string; // Optional field
    purchasePlanningId: number; // Foreign key to Purchase Planning
    purchaseRawProductDetails: PurchaseRawProductDetail[]
}
export class PurchaseRawProductDetail extends BaseAuditable {
    id: number;
    inputProductCode: number | null;  
    qty: number | null;  
    vendorCode: number; // Foreign key to Vendor Master
    leadTime: number; // Lead Time (Days) - Manual Entry
    purchaseOrderDate: Date; // Manual Entry
    purchaseHour: number;
    materialReceievedDate: Date; // Manual Entry
    uom: string | null;  
    drawingNumber: string | null; 
    purchasePlanningId: number | null;
}