import { DocumentDetails } from "./document-details";

export class AuditLog {
    id: number;
    pageName: string;
    userId: string;
    actionDateTime: Date;
    actionType: string;
    fieldName: string;
    oldValue: string;
    newValue: string;
    entityName: string;
    entityId: string;
}