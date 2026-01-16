import { BaseAuditable } from "./base-auditable";

export class ProjectAssignment extends BaseAuditable {
    id: number;
    effectiveDate: Date;
    projectAssignmentCode: string;
    statusId: number;
    projectId: number;
    projectCode: string;
    projectName: string;
    locationId: number;
    designationIds: string;
    isPublish?: boolean;
    isApprove?: boolean;
    isReject?: boolean;
    createdBy?: number;
    createdDate?: Date;
    isActive: boolean = true;

    projectDesignationUsers?: ProjectDesignationUser[];
    modifiedBy: any;
}

export class ProjectDesignationUser extends BaseAuditable {
    id: number;
    projectAssignmentId: number;
    designations: number; 
    assignedUserIds: string;
    isActive: boolean = true;
   // designation?: Designation;
}
