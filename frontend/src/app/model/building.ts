import { BaseAuditable } from './base-auditable';

export class Building extends BaseAuditable {
    id: number;
    code: string;
    buildingName: string;
    propertyId: number;
    deviceId?: number;
    deviceUserName?: string;
    devicePassword?: string;
    isActive: boolean;
}
