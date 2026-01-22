import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { AmenitySlotTemplate } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class AmenitySlotTemplateService extends CRUDService<AmenitySlotTemplate> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'amenity-slot-template');
    }

    getSlotTemplates(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.AmenitySlotTemplateListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getSlotTemplateById(id: number) {
        return this.http.get(`${APIConstant.AmenitySlotTemplateGetById}?id=${id}`);
    }

    addSlotTemplate(template: AmenitySlotTemplate) {
        return this.http.post(`${APIConstant.AmenitySlotTemplateAdd}`, template);
    }

    addSlotTemplates(templates: AmenitySlotTemplate[]) {
        return this.http.post(`${APIConstant.AmenitySlotTemplateAddBulk}`, templates);
    }

    updateSlotTemplate(template: AmenitySlotTemplate) {
        return this.http.post(`${APIConstant.AmenitySlotTemplateEdit}`, template);
    }

    upsertSlotTemplates(templates: AmenitySlotTemplate[]) {
        return this.http.post(`${APIConstant.AmenitySlotTemplateEditBulk}`, templates);
    }

    deleteSlotTemplate(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.AmenitySlotTemplateDelete}?id=${id}`);
    }

    getAmenities() {
        return this.http.get(`${APIConstant.AmenityMasterBasicList}`);
    }
}
