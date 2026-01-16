import { BaseAuditable } from './base-auditable';

export class State extends BaseAuditable {
    id: number;
    name: string;
    countryId: number;
}