export interface ParentGroup {
    id?: number;
    name: string;
    type: 'Income' | 'Expense';
    priority: number;
    isActive: boolean;
    serviceCategoryId: number;
    groupEntryId: number;
    subGroups: SubGroup[];
    lineItems: LineItem[];
}
export interface SubGroup {
    id?: number;
    name: string;
    priority: number;
    isActive: boolean;
    parentGroupId: number;
    lineItems: LineItem[];
}
export interface LineItem {
    id?: number;
    name: string;
    priority: number;
    isActive: boolean;
    parentGroupId: number;
    subGroupId: number; 
    type:string;
    parentType?:number;
    itemType:number;
    actualmappingType:string;
}
