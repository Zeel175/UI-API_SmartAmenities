import { Time } from "@angular/common";
import { BaseAuditable } from "./base-auditable";


export class ProductionWorkDone extends BaseAuditable {
    id: number;
    processCode: number;
    planningDetailId:number;
    date: string;
    actualDate: string;
    startTime: string;
    endTime: string;
    totalHours: string;
    actualCompletedQty: number;
    remarks: string; 
    serialNumbers: string;
    processName?: any;
    rejected?: any;
}
