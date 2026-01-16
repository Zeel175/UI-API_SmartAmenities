import { BaseAuditable } from "./base-auditable";

export class Workflow extends BaseAuditable {
    id: number;
    workflowCode: string;
    workflowDate: Date;
    transactionId: number;
    remarks: string;

    workflowLevels?: WorkflowLevel[];
}

export class WorkflowLevel extends BaseAuditable {
    id: number;
    workflowId: number;
    levelNo: number;
    approverTypeId: number;
    userIds?: number[];

    workflowUsers?: WorkflowUser[];
}

export class WorkflowUser extends BaseAuditable {
    id?: number;
    workflowId?: number;
    workflowLevelId?: number;
    userId: number;
}