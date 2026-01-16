import { BaseAuditable } from './base-auditable';

export class Module extends BaseAuditable {
  id: number;
  name: string;
  code: string;
  priority: number;
}
