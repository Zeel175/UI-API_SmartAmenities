import { BaseAuditable } from "./base-auditable";

// Model for Process
export class Process extends BaseAuditable {
    id: number;
    productCode: number;
    processName: string;
    priority: number;
    processDuration: number;
    processStartDate: Date; // Ensure this matches the payload structure
    processEndDate: Date; // Ensure this matches the payload structure
    remarks?: string;
    isActive: boolean;

    // Arrays of related entities
    processQCParameters: QcParameter[];
    processMachines: ProcessMachine[];
planningDetailId: any;
startDate?: string|number|Date;
qtyCompleted?: number;
    productName: any;
}

// Model for QcParameter
export class QcParameter extends BaseAuditable {
    id: number;
    qcParameterName: string; // Ensure this matches the payload structure
    processId: number; // Ensure this matches the payload structure
}

// Model for ProcessMachine
export class ProcessMachine extends BaseAuditable {
    id: number;
    machineCode: number; // Ensure this matches the payload structure
    processId: number; // Ensure this matches the payload structure
}