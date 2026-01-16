import { BaseAuditable } from "./base-auditable";

// Main Production Work Order class
export class ProductionWorkOrder extends BaseAuditable {
    id: number;
    workOrderCode: string; // Auto-generated code
    workOrderDate: Date; // System Date Default
    customerCode?: number; // Foreign key to Customer Master
    customerPONumber?: string; // Optional field
    customerPODate?: Date; // Required
    remarks: string; // Optional field
    statusId: number;
    status?:string;
    isApprove: boolean;
    isReject: boolean;
    productionWorkOrderDetails: ProductionWorkOrderDetail[]; // Details array

    constructor() {
        super();
        this.productionWorkOrderDetails = []; // Initialize the details array
    }
}

// Details related to Production Work Order
export class ProductionWorkOrderDetail extends BaseAuditable {
    id: number;
    productCode: number; // Foreign key to Product/BOM Master
    qty: number; // Manual Entry
    typeOfConnection: string; // Manual Entry
    colour: string; // Manual Entry
    packing: string; // Manual Entry
    serialNumberFrom: string; // Serial Number From
    serialNumberTo: string; // Serial Number To
    expectedDeliveryDate: Date; // Manual Entry
    productionWorkOrderId: number; // Foreign key to Production Work Order
    productName: any;

    constructor() {
        super();
    }
}
