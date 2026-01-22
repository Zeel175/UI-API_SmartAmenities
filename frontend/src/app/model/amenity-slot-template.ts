export class AmenitySlotTemplate {
    id: number;
    amenityId: number;
    amenityName?: string;
    dayOfWeek: string;
    startTime: string;
    endTime: string;
    slotDurationMinutes: number;
    bufferTimeMinutes?: number;
    capacityPerSlot?: number;
    isActive: boolean;
}
