import { BaseAuditable } from './base-auditable';

export class GroupCode extends BaseAuditable {
    id: number;
    code: string;
    name: string;
    groupName: string;
    priority: number;
    value?:string;
}