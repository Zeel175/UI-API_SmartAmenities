import { BaseAuditable } from "./base-auditable";
import { Permission } from "./permission";

export class Attachment extends BaseAuditable {
    id: number;
    filePath : string;
    fileName : string;
    budgetId : number;
}