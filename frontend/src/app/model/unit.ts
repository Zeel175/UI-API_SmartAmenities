import { BaseAuditable } from './base-auditable';

export class Unit extends BaseAuditable {
    id: number;
    code: string;
    unitName: string;
    buildingId: number;
    floorId: number;
    occupancyStatusId: number;
    isActive: boolean;
}