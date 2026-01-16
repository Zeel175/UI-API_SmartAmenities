import { BaseAuditable } from "./base-auditable";

// Main Production Planning class
export class ProductionPlanning extends BaseAuditable {
    id: number;
    planningCode: string; // Auto-generated code
    planningDate: Date;
    customerName?:string;
    customerCode: number; // Foreign key to User entity (Customer)
    customerPONumber: string; // Optional field
    customerPODate: Date;
    productCode: number;
    productName?: string;
    qty: number;
    uom: string; // Unit of Measure
    remark: string; // Optional field
    statusId:string;
    status?:string;
    workDoneIds:string;
    isApprove: boolean;
    isReject: boolean;
    isDraft?: boolean;
    productionPlanningDetails: ProductionPlanningDetail[]; // Details array
    processCode: string;
    processName: string
}

// Details related to Production Planning
export class ProductionPlanningDetail extends BaseAuditable {
    id: number;
    processCode: number; // Foreign key to Process entity
    processName:string;
    machineCode?: number;
    machineName?:string;
    userCode: number;
    userName?:string;
    startDate: Date;
    endDate: Date;
    qtyCompleted: number;
    remark: string; // Optional field
    productionPlanningId: number; // Foreign key to Production Planning
    isMore?: boolean;
}
