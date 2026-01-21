import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { AmenityMaster } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AmenityMasterService extends CRUDService<AmenityMaster> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'amenity-master');
    }

    getAmenities(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.AmenityMasterListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getAmenityById(id: number) {
        return this.http.get(`${APIConstant.AmenityMasterGetById}?id=${id}`);
    }

    addAmenity(amenity: any) {
        return this.http.post(`${APIConstant.AmenityMasterAdd}`, amenity);
    }

    updateAmenity(amenity: any) {
        return this.http.post(`${APIConstant.AmenityMasterEdit}`, amenity);
    }

    deleteAmenity(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.AmenityMasterDelete}?id=${id}`);
    }

    getBuildings() {
        return this.http.get(`${APIConstant.BuildingBasicList}`);
    }

    getFloors() {
        return this.http.get(`${APIConstant.FloorBasicList}`);
    }
}
