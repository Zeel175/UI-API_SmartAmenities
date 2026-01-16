import { List } from "./list";

export class PermissionModule {
    id: number;
    name: string;
    code: string;
    hasAllAccess?: boolean;
    hasAllMasterAccess?: boolean;
    moduleGroups: PermissionModuleGroup[];
}


export class PermissionModuleGroup {
    id: number;
    name: string;
    code: string;
    permissions: GroupPermission[];
}

export class GroupPermission extends List {
    hasAccess: boolean;
    hasMasterAccess: boolean;
}

export class Permission {
    id: number;
    hasMasterAccess: boolean;
}