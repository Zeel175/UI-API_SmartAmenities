import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant } from 'app/core';

@Injectable({
    providedIn: 'root'
})
export class DeviceService {
    constructor(private http: HttpClient) {}

    getDevices() {
        return this.http.get(`${APIConstant.HikvisionDevicesList}`);
    }
}
