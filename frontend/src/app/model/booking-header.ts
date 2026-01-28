export interface BookingUnit {
    id?: number;
    bookingId?: number;
    amenityUnitId?: number;
    unitNameSnapshot?: string;
    capacityReserved?: number;
    bookingSlot?: BookingSlot | null;
}

export interface BookingSlot {
    id?: number;
    bookingId?: number;
    bookingUnitId?: number;
    amenityId?: number;
    amenityUnitId?: number;
    slotStartDateTime?: string;
    slotEndDateTime?: string;
    slotStatus?: string;
    checkInRequired?: boolean;
    checkInTime?: string;
    checkOutTime?: string;
}

export interface BookingSlotAvailability {
    id?: number;
    slotStartDateTime: string;
    slotEndDateTime: string;
    capacityPerSlot: number;
    availableCapacity: number;
    slotCharge?: number;
    isChargeable: boolean;
    chargeType?: string;
    baseRate?: number;
    securityDeposit?: number;
    refundableDeposit: boolean;
    taxApplicable: boolean;
    taxCodeId?: number;
    taxPercentage?: number;
}

export class BookingHeader {
    id?: number;
    amenityId?: number;
    amenityName?: string;
    societyId?: number;
    societyName?: string;
    bookingNo?: string;
    bookingDate?: string;
    remarks?: string;
    status?: string;
    residentUserId?: number;
    flatId?: number;
    residentNameSnapshot?: string;
    contactNumberSnapshot?: string;
    isChargeableSnapshot?: boolean;
    amountBeforeTax?: number;
    taxAmount?: number;
    depositAmount?: number;
    discountAmount?: number;
    convenienceFee?: number;
    totalPayable?: number;
    requiresApprovalSnapshot?: boolean;
    approvedBy?: number;
    approvedOn?: string;
    rejectionReason?: string;
    cancelledBy?: number;
    cancelledOn?: string;
    cancellationReason?: string;
    refundStatus?: string;
    createdDate?: string;
    createdBy?: number;
    modifiedDate?: string;
    modifiedBy?: number;
    bookingUnits?: BookingUnit[];
}
