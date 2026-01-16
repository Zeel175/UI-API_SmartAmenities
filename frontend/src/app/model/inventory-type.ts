import { BaseAuditable } from "./base-auditable";

export class InventoryType extends BaseAuditable {
    id: number;
    inventoryTypeCode: string;
    inventoryTypeName: string;
    isInward: boolean;
    inwardType: string
    applicableToId: number;
    applicableToName: string;
    isSales?: boolean;
    sales: string
    isPurchase?: boolean;
    purchase: string
    isReserve?: boolean;
    reserve: string
    isAdmin?: boolean;
    admin: string
}