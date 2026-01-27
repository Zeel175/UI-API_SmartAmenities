import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { Unit } from 'app/model/unit';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class UnitService extends CRUDService<Unit> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'unit');
    }

    getUnits(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.UnitMasterList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getAllUnits() {
        return this.http.get(`${APIConstant.UnitMasterList}`);
    }

    deleteUnit(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.UnitMasterDelete}?id=${id}`);
    }

    getUnitById(id: number) {
        return this.http.get(`${APIConstant.UnitMasterGetById}?id=${id}`);
    }

    addUnit(unit: Unit) {
        return this.http.post(`${APIConstant.UnitMasterAdd}`, unit);
    }

    updateUnit(unit: Unit) {
        return this.http.post(`${APIConstant.UnitMasterEdit}`, unit);
    }

    getBuildings() {
        return this.http.get(`${APIConstant.BuildingBasicList}`);
    }

    getUnitsByBuilding(buildingId: number) {
        return this.http.get(`${APIConstant.UnitByBuilding}?buildingId=${buildingId}`);
    }

   
    getOccupancyStatuses(): Observable<any> {
    return this.http.get(`${APIConstant.GroupCodeListByGroupName}/occupancystatus`);
}

}
