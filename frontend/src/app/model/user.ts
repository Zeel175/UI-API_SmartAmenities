export class User {
    id: number;
    userName: string;
    firstName: string;
    name?: string;
    lastName: string;
    email: string;
    phoneNumber: string;
    employeeCode: string;
    password?: string;
    confirmPassword?: string;
    designationId?: number;
    reportingManager?: string;
    reportsTo?: Array<{ reportToId: number, reportToName: string }> = [];
    roles?: Array<{ roleId: number, roleName: string, code: string }> = [];
    uploadedFiles?: File[];
    createdBy?: number;
    modifiedBy?: number;
    createdDate: Date;
    modifiedDate: Date;
}

export class BrowserUser {
    '.expires': Date;
    '.issued': Date;
    access_token: string;
    expires_in: number;
    permissions: string;
    token_type: string;
    tokenId: string;
    user: string;
}

export class LoginUser {
    email: string;
    password: string;
}

