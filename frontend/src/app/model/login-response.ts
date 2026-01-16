export interface LoginResponse {
    id: number;
    token: string;
    displayName: string;
    expireAt: string;
    userImageFilePath?: string | null;
    isAdmin: boolean;
    signaturePath?: string | null;
    isSuccess: boolean;
    message: string;
    role: Array<{
        id: number;
        name: string;
        normalizedName: string;
        concurrencyStamp?: string | null;
    }>;
    permissions: string[];
    rolePermissions: any[];  // adapt as needed
    user: {
        name: string;
        lastName?: string | null;
        isActive: boolean;
        createdDate: string;
        createdBy?: any;
        modifiedDate?: any;
        modifiedBy?: any;
        employeeCode?: string | null;
        isAdmin: boolean;
        designationId?: number | null;
        designation?: any;
        reportingManager?: any;
        projectMasters?: any;
        workflow?: any;
        approver?: any;
        projectAssignments?: any;
        projectDesignationUser?: any;
        id: number;
        userName: string;
        normalizedUserName?: string | null;
        email: string;
        normalizedEmail?: string | null;
        emailConfirmed: boolean;
        passwordHash?: string | null;
        securityStamp?: string | null;
        concurrencyStamp: string;
        phoneNumber?: string | null;
        phoneNumberConfirmed: boolean;
        twoFactorEnabled: boolean;
        lockoutEnd?: any;
        lockoutEnabled: boolean;
        accessFailedCount: number;
    }
}
