export class AmenityDocument {
    id: number;
    fileName: string;
    filePath: string;
    contentType: string;
}

export class AmenityMaster {
    id: number;
    name: string;
    code?: string;
    type: string;
    description?: string;
    buildingId: number;
    buildingName?: string;
    floorId: number;
    floorName?: string;
    location?: string;
    status: string;
    maxCapacity?: number;
    maxBookingsPerDayPerFlat?: number;
    maxActiveBookingsPerFlat?: number;
    minAdvanceBookingHours?: number;
    minAdvanceBookingDays?: number;
    maxAdvanceBookingDays?: number;
    bookingSlotRequired?: boolean;
    slotDurationMinutes?: number;
    bufferTimeMinutes?: number;
    allowMultipleSlotsPerBooking?: boolean;
    allowMultipleUnits?: boolean;
    requiresApproval?: boolean;
    allowGuests?: boolean;
    maxGuestsAllowed?: number;
    availableDays?: string;
    openTime?: string;
    closeTime?: string;
    holidayBlocked?: boolean;
    maintenanceSchedule?: string;
    isChargeable?: boolean;
    chargeType?: string;
    baseRate?: number;
    securityDeposit?: number;
    refundableDeposit?: boolean;
    taxApplicable?: boolean;
    taxCodeId?: number;
    taxPercentage?: number;
    termsAndConditions?: string;
    documentDetails?: AmenityDocument[];
}
