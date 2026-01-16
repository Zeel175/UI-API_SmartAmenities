export class List {
    id: number;
    name: string;
    code?: string;
    purchaseOrderCode?: string;
     description?: string; 
    allowToChange?: boolean;
    categoryName?: string;
    typeName?: string;
     isActive?: boolean;
}
// For Terms & Conditions only:
export class TermsConditionList extends List {
    allowToChange: boolean;
}