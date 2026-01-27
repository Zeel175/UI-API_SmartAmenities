export interface BookingUnit {
    id?: number;
    bookingId?: number;
    amenityUnitId?: number;
    unitNameSnapshot?: string;
    capacityReserved?: number;
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
