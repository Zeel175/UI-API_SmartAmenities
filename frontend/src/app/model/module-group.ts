import { BaseAuditable } from './base-auditable';

export class ModuleGroup extends BaseAuditable {
    id: number;
    code: string;
    name: string;
    moduleId: number | null;
    priority: number;
    createdBy: number | null;
    createdOn: Date;
    modifiedBy: number | null;
    modifiedOn: Date | null;
}