import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { AmenityUnit } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AmenityUnitMasterService extends CRUDService<AmenityUnit> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'amenity-unit-master');
    }

    getAmenityUnits(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.AmenityUnitListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getAmenityUnitById(id: number) {
        return this.http.get(`${APIConstant.AmenityUnitGetById}?id=${id}`);
    }

    addAmenityUnit(unit: AmenityUnit) {
        return this.http.post(`${APIConstant.AmenityUnitAdd}`, unit);
    }

    updateAmenityUnit(unit: AmenityUnit) {
        return this.http.post(`${APIConstant.AmenityUnitEdit}`, unit);
    }

    deleteAmenityUnit(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.AmenityUnitDelete}?id=${id}`);
    }

    getAmenities() {
        return this.http.get(`${APIConstant.AmenityMasterBasicList}`);
    }
}
