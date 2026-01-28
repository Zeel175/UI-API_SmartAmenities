export interface AmenitySlotTemplateTime {
    startTime: string;
    endTime: string;
    capacityPerSlot?: number;
    slotCharge?: number;
    isChargeable?: boolean;
    chargeType?: string;
    baseRate?: number;
    securityDeposit?: number;
    refundableDeposit?: boolean;
    taxApplicable?: boolean;
    taxCodeId?: number;
    taxPercentage?: number;
    isActive?: boolean;
}

export class AmenitySlotTemplate {
    id: number;
    amenityId: number;
    amenityUnitId?: number | null;
    amenityName?: string;
    dayOfWeek: string;
    slotDurationMinutes: number;
    bufferTimeMinutes?: number;
    isActive: boolean;
    slotTimes?: AmenitySlotTemplateTime[];
}
