export class AmenityUnitFeature {
    featureId?: number;
    featureName: string;
    isActive?: boolean;
}

export class AmenityUnit {
    id: number;
    amenityId: number;
    amenityName?: string;
    unitName: string;
    unitCode?: string;
    deviceId?: number;
    deviceUserName?: string;
    devicePassword?: string;
    status: string;
    shortDescription?: string;
    longDescription?: string;
    isChargeable?: boolean;
    chargeType?: string;
    baseRate?: number;
    securityDeposit?: number;
    refundableDeposit?: boolean;
    taxApplicable?: boolean;
    taxCodeId?: number;
    taxPercentage?: number;
    features?: AmenityUnitFeature[];
}
