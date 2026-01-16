import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { Zone } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ZoneService extends CRUDService<Zone> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'zone');
    }

    getZones(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.ZoneMasterList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getAllZoneBasic() {
        return this.http.get(`${APIConstant.ZoneBasicList}`);
    }

    getBuildings() {
        return this.http.get(`${APIConstant.BuildingBasicList}`);
    }

    getZoneById(id: number) {
        return this.http.get(`${APIConstant.ZoneMasterGetById}?id=${id}`);
    }

    addZone(zone: Zone) {
        return this.http.post(`${APIConstant.ZoneMasterAdd}`, zone);
    }

    updateZone(zone: Zone) {
        return this.http.post(`${APIConstant.ZoneMasterEdit}`, zone);
    }

    deleteZone(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.ZoneMasterDelete}?id=${id}`);
    }
}
