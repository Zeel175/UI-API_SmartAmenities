import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { Building } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class BuildingService extends CRUDService<Building> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'building');
    }

    getBuilding(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.BuildingMasterList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    deleteBuilding(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.BuildingMasterDelete}?id=${id}`);
    }

    getBuildingById(id: number) {
        return this.http.get(`${APIConstant.BuildingMasterGetById}?id=${id}`);
    }

    addBuilding(building: Building) {
        return this.http.post(`${APIConstant.BuildingMasterAdd}`, building);
    }

    updateBuilding(building: Building) {
        return this.http.post(`${APIConstant.BuildingMasterEdit}`, building);
    }
    getHikvisionDevices() {
        return this.http.get(`${APIConstant.HikvisionDevicesList}`);
    }
}