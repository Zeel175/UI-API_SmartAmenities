import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { Floor } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class FloorService extends CRUDService<Floor> {

    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'floor');
    }

    getFloor(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.FloorMasterList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    deleteFloor(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.FloorMasterDelete}?id=${id}`);
    }

    getFloorById(id: number) {
        return this.http.get(`${APIConstant.FloorMasterGetById}?id=${id}`);
    }

    addFloor(floor: any) {
        return this.http.post(`${APIConstant.FloorMasterAdd}`, floor);
    }

    updateFloor(floor: any) {
        return this.http.post(`${APIConstant.FloorMasterEdit}`, floor);
    }

    getBuildings() {
        return this.http.get(`${APIConstant.BuildingBasicList}`);
    }
    getFloors() {
        return this.http.get(`${APIConstant.FloorBasicList}`);
    }

}