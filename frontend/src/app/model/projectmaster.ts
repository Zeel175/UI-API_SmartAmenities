import { BaseAuditable } from './base-auditable';

export class ProjectMaster extends BaseAuditable {
    id?: number;
    projectCode: string;
    projectName: string;
    startDate: Date;
    endDate: Date;
    locationId?: number;
    location?: Location;
    manpowerQuantity?: number;
    address: string;
    serviceCategoryId?: number;
    createdBy?: number;
    modifiedBy?: number;
    createdDate?: Date;
    modifiedDate?: Date;
    // employeeIds: number[] = [];
    isActive: boolean = true;
}
