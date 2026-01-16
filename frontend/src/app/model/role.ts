import { BaseAuditable } from "./base-auditable";
import { Permission } from "./permission";

export class Role extends BaseAuditable {
    id: number;
    name: string;
    Permissions: Permission[]
    rolePermissions:Permission[]
    assignedPermissions: any;
    NormalizedName:string;
}