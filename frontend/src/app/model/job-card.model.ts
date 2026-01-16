import { BaseAuditable } from './base-auditable';

export class JobCard extends BaseAuditable {
  id: number;
  jobTitle: string;
  assignedTo: string;
  dueDate: Date;
  status: string;
  priority: number;
}
