import { BaseAuditable } from "./base-auditable";
import { ModuleGroup } from "./module-group"; 
import { User } from "./user"; 
import { GroupCode } from "./group-code";

export class Approval extends BaseAuditable {
  id: number;
  moduleGroupId: number;
  userId: number;
  approverTypeId: number;
  level: number;
  status: boolean;
  mgEntryId:number;
  decisionRemarks?: string;
  moduleGroup?: ModuleGroup;
  user?: User;
  approverType?: GroupCode;
    userName?: string;
}

