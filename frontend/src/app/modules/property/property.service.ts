import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { APIConstant, BaseService, CRUDService } from "app/core";
import { List } from "app/model";
import { Property } from "app/model/property";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'  // This makes CustomerService available application-wide
})
export class PropertyService extends CRUDService<Property>{

     constructor(private _baseService: BaseService,private http: HttpClient) {
        super(_baseService, "property");
    }
    getProperty(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.PropertyMasterList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }
    // deleteProperty(id: number): Observable<any> {
    //     return this._baseService.delete(`${APIConstant.PropertyMasterList}/${id}`);
    // }
    deleteProperty(id: number): Observable<void> {
   // Hit the correct DELETE route, passing id as query param
   return this.http.delete<void>(
     `${APIConstant.PropertyMasterDelete}?id=${id}`
   );
 }
    getPropertyById(id: number) {
        return this.http.get(`${APIConstant.PropertyMasterGetById}?id=${id}`);
    }
    addProperty(property: Property) {
        return this.http.post(`${APIConstant.PropertyMasterAdd}`, property);
    }
    updateProperty(property: Property) {
        return this.http.post(`${APIConstant.PropertyMasterEdit}`, property);
    }
    getAllPropertyBasic(): Observable<Property[]> {
        return this.http.get<Property[]>(`${APIConstant.PropertyBasicList}`);
    }


}