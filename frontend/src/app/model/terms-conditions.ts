import { BaseAuditable } from './base-auditable';

export class TermsAndConditions extends BaseAuditable {
    id: number;
    code: string;
    name: string;
    description: string;
    priority: number;
    allowToChange: boolean;
    typeId: number;
    typeName?: string;
    categoryId?: number;
    categoryName?: string;
    createdBy: number | null;
    createdOn: Date;
    modifiedBy: number | null;
    modifiedOn: Date | null;
}