import { BaseAuditable } from './base-auditable';

export class ProductGroup extends BaseAuditable {
    id: number;
    code: string;
    name: string;
    groupName: string;
    //priority: number;
    //value?:string;
}