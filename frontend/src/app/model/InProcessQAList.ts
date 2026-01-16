// in-process-qa.model.ts (recommended location)

export interface InProcessQAList {
    id: number;
    inProcessQACode: string | null;
    productionWorkDoneId: number;
    processCode: number;
    processName: string;
    date: string;
    startTime: string;
    endTime: string;
    totalHours: string;
    actualCompletedQty: number;
    planningDetailId: number;
    customerName: string;
    productName: string;
    productModelNumber:string;
    customerPONumber: string;
    customerPODate: string;
    isActive: boolean;
    rowVersion: string | null;
    // Add remarks if it's coming from API (not in example, add if needed)
    remarks?: string;
}
