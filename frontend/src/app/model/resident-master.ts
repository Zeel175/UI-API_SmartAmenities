import { BaseAuditable } from './base-auditable';

export class ResidentMaster extends BaseAuditable {
    id: number;
    code: string;
    parentFirstName: string;
    parentLastName: string;
    email?: string;
    mobile?: string;
    faceId?: string;
    faceImageBase64?: string;
    faceImageContentType?: string;
    fingerId?: string;
    cardId?: string;
    profilePhoto?: string;
    qrId?: string;
    unitIds: number[] = [];
    isActive: boolean;
    isResident?: boolean;
    familyMembers: ResidentFamilyMember[] = [];
}

export class ResidentFamilyMember extends BaseAuditable {
    id: number;
    code?: string;
    residentMasterId: number;
    unitIds: number[] = [];
    firstName: string;
    lastName: string;
    email?: string;
    mobile?: string;
    faceId?: string;
    faceImageBase64?: string;
    faceImageContentType?: string;
    fingerId?: string;
    cardId?: string;
    qrId?: string;
    isActive: boolean;
    isResident?: boolean;
    profilePhoto?: string;          // saved path (edit mode)
    profilePhotoFile?: File | null;
}
