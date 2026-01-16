
export interface HRProjectwiseBudgetPlanningHeader {
  id: number;
  uniqueID: string;
  budgetCode: string;
  budgetDate?: string;
  budgetMonth?: number;
  budgetYear?: number;
  remarks?: string;
  projectID?: number;
  hrProjectwiseBudgetPlanningID?: number;
  isActive: boolean;
  stateID?: number;
  createdBy?: number;
  createdOn?: string;
  lastModifiedBy?: number;
  lastModifiedOn?: string;
  budgetType?: number;
  details: HRProjectwiseBudgetPlanningDetail[];
}
export interface HRProjectwiseBudgetPlanningDetail {
  id: number;
  uniqueID: string;
  hrProjectwiseBudgetPlanningHeaderID: number;
  usedBudget: number;
  balanceBudget: number;
  incrementAmount: number;
  decrementAmount: number;
  projectID: number;
  remarks?: string;
  isActive: boolean;
  stateID?: number;
  createdBy?: number;
  createdOn?: string;
  lastModifiedBy?: number;
  lastModifiedOn?: string;
}
