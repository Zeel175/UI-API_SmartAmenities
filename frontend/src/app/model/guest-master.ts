import { BaseAuditable } from './base-auditable';

export class GuestMaster extends BaseAuditable {
    id: number;
    code: string;
    firstName: string;
    lastName: string;
    email?: string;
    mobile?: string;
    faceId?: string;
    fingerId?: string;
    cardId?: string;
    profilePhoto?: string;
    qrId?: string;
    unitId: number;
    isActive: boolean;
    isResident?: boolean;
    familyMembers: GuestFamilyMember[] = [];
}

export class GuestFamilyMember extends BaseAuditable {
    id: number;
    guestMasterId: number;
    unitIds: number[] = [];
    firstName: string;
    lastName: string;
    email?: string;
    mobile?: string;
    faceId?: string;
    fingerId?: string;
    cardId?: string;
    qrId?: string;
    isActive: boolean;
    isResident?: boolean;
    profilePhoto?: string;
    profilePhotoFile?: File | null;
}
