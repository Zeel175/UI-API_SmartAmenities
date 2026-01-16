import { BaseAuditable } from "./base-auditable";

export class CustomerWithPONumbers extends BaseAuditable {
       id: number;
       name: string;
       poNumbers: {
           salesOrderCode: string;
           customerPONumber: string;
           date: string;
           totalQty: number;
       }[];
  isSelected: unknown;
}