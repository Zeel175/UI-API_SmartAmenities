import { BaseAuditable } from "./base-auditable";
export class InProcessQA extends BaseAuditable {
  id: number;
  inProcessQACode: string;
  productionWorkDoneId: number;
  entries: Entry[];
  isCompleted: any;

  constructor() {
    super();
    this.entries = [];
  }
}

export class Entry {
  serialNumber: string;
  processId: number;
  qcParameters: QCParameter[];
  status: boolean;
  remarks: string;

  constructor(serialNumber: string, qcParameters: QCParameter[], remarks: string, status: boolean) {
    this.serialNumber = serialNumber;
    this.qcParameters = qcParameters;
    this.remarks = remarks;
    this.status = status;
  }
}

export class QCParameter {
  qcParameterValue: string;
  value: number; // Unique value for each QC parameter

  constructor(qcParameterValue: string, value: number) {
    this.qcParameterValue = qcParameterValue;
    this.value = value;
  }
}
