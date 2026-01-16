import { BaseAuditable } from "./base-auditable";
import { Inward } from "./inward";
import { Product } from "./product";
import { PurchaseOrder } from "./purchase-order";
import { VendorMaster } from "./vendor-master";

export class Inspection extends BaseAuditable {
    id: number;
    purchaseOrderId: number;
    inwardId: number;
    vendorId: number;
    productId: number;
    drawingNo: string;
    inspectedDate: Date;
    receivedQty: number;
    inspectedQty: number;
    purchaseOrder: PurchaseOrder;
    inward: Inward;
    vendor: VendorMaster;
    product: Product;
    inspectionObservations: InspectionObservation[];
    productName?: string;
    inwardName?: string;
    vendorName?: string;

    constructor() {
        super();
        this.inspectionObservations = [];
    }
}

export class InspectionObservation extends BaseAuditable {
    id: number;
    inspectionId: number;
    dimension: string | null;
    tolerance: string | null;
    observations: string | null;
    maxVariation: number | null;
    minVariation: number | null;
    status: boolean | null;
    inspection: Inspection;
}