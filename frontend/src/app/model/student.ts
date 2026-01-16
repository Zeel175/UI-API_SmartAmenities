import { BaseAuditable } from './base-auditable';

export class Student extends BaseAuditable {
  id: number;
  studentName: string;
  studentNumber: string;
}
