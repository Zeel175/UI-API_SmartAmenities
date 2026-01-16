import { BaseAuditable } from './base-auditable';

export class Town extends BaseAuditable {
    id: number;
    name: string;
    alias: string;
    contactPerson: string;
    addressLine1: string;
    addressLine2: string;
    apartment: string;
    city: string;
    stateId?: number;
    stateName: string;
    countryId?: number;
    countryName: string;
    zipCode: string;
    website: string;
    email: string;
    phoneNumber: string;
    faxNumber: string;
    extension: string;
    allowActiveUsers: number;
    timeZoneId: number;
    isHotlistApplicable?: boolean;
}