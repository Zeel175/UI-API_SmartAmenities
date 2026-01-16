import { BaseAuditable } from "./base-auditable";

export class ServiceCategory extends BaseAuditable {
    id: number;
    categoryName: string;
    description?: string;
    // isActive: boolean = true;
}
