export interface AmenitySlotTemplateTime {
    startTime: string;
    endTime: string;
    capacityPerSlot?: number;
    isActive?: boolean;
}

export class AmenitySlotTemplate {
    id: number;
    amenityId: number;
    amenityName?: string;
    dayOfWeek: string;
    slotDurationMinutes: number;
    bufferTimeMinutes?: number;
    isActive: boolean;
    slotTimes?: AmenitySlotTemplateTime[];
}
