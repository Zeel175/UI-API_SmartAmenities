import { BaseAuditable } from './base-auditable';

export class Customer extends BaseAuditable {
    id: number;
    customerCode: string;
    customerName: string;
    customerCategoryId: number;
    industryId: number;
    customerTypeId: number;
    sourceId: number;
    phoneNo: string;
    website?: string;
    gstinNo?: string;
    customerDetails: CustomerDetails[];
    customerContactDetails: CustomerContactDetails[];
    customerUserDetails: CustomerUserDetails[];
    customerTerritoryDetails: CustomerTerritoryDetails[];
     name: any;
}

export class CustomerDetails extends BaseAuditable {
    id: number;
    customerId: number;
    location: string;
    street?: string;
    city: string;
    state: string;
    country: string;
    zipCode?: string;
    gstinNo?: string;
}

export class CustomerContactDetails extends BaseAuditable {
    id: number;
    customerId: number;
    contactName: string;
    jobTitle: string;
    email: string;
    phoneNo: string;
    mobile1: string;
    mobile2?: string;
    department?: string;
    designation?: string;
}

export class CustomerUserDetails extends BaseAuditable {
    id: number;
    customerId: number;
    userId: number;
}

export class CustomerTerritoryDetails extends BaseAuditable {
    id: number;
    customerId: number;
    territoryId: number;
}