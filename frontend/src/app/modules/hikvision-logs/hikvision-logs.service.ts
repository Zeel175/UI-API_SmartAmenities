import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant } from 'app/core';
import { Observable } from 'rxjs';

export interface HikvisionLogsRequest {
    start: string;
    end: string;
    buildingId: number;
    unitId: number;
    userIds: number[];
    guestIds: number[];
}

@Injectable({
    providedIn: 'root'
})
export class HikvisionLogsService {
    constructor(private http: HttpClient) {}

    searchLogs(payload: HikvisionLogsRequest): Observable<unknown> {
        return this.http.post(APIConstant.HikvisionLogsSearch, payload);
    }
}
