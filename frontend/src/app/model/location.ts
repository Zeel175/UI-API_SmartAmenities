import { BaseAuditable } from "./base-auditable";

export class Location extends BaseAuditable {
    id: number;
    locationCode: string;
    locationName: string;
    companyId: number;
    companyName: string;
    addressLine1: string;
    addressLine2?: string;
    addressLine3?: string;
    city: string;
    cityId: number;
    stateId: number;
    stateName: string;
    countryId: number;
    countryName: string;
    pincode: string;
    createdBy?: string;
    modifiedBy?: string;
    locationWarehouseDetails: LocationWarehouseDetail[];
}

export class LocationWarehouseDetail extends BaseAuditable {
    id: number;
    locationId: number;
    warehouseName: string;
    addressLine1: string;
    addressLine2?: string;
    addressLine3?: string;
    city: string;
    stateId: number;
    stateName: string;
    countryId: number;
    countryName: string;
    pincode: string;
}