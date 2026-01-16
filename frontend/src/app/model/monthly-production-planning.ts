import { BaseAuditable } from "./base-auditable";

// Main Production Planning class
export class monthlyProductionPlanning extends BaseAuditable {
    id: number;
    planningCode: string; // Auto-generated code
    planningDate: Date;
    customerName?:string;
    customerCode: number; // Foreign key to User entity (Customer)
    customerPONumber: string; // Optional field
    customerPODate: Date;
    productCode: number; // Foreign key to Product entity
    qty: number;
    uom: string; // Unit of Measure
    remark: string; // Optional field
    statusId:string;
    workDoneIds:string;
    productionPlanningDetails: MonthlyProductionPlanningDetail[]; // Details array
    processCode: string;
    processName: string
}
export class MonthlyProductionPlanningDetail extends BaseAuditable {
    id: number;
    processCode: number; // Foreign key to Process entity
    processName:string;
    machineCode: number; // Foreign key to Machine entity
    userCode: number; // Foreign key to User entity (Operator)
    startDate: Date;
    endDate: Date;
    qtyCompleted: number;
    remark: string; // Optional field
    productionPlanningId: number; // Foreign key to Production Planning
}