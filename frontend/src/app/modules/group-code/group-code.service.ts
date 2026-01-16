import { Injectable } from "@angular/core";
import { CRUDService, BaseService } from "app/core";
import { GroupCode } from "app/model";
import { APIConstant } from "app/core";
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";
@Injectable({
    providedIn: 'root'
})
export class GroupCodeService extends CRUDService<GroupCode> {
    constructor(private _baseService: BaseService,private http: HttpClient) {
        super(_baseService, "groupcode");
    }
    //  generateCode() {
    //     return this._baseService.get<string>(`${APIConstant.groupcode}/generateCode`);
    // }
    addGroupCode(model: GroupCode) {
  return this._baseService.post<number>(   // ⬅️ number, not GroupCode
    `${APIConstant.groupcode}/AddGroupCode`,
    model
  );
}
    getGroupCodes(pageIndex: number, pageSize: number) {
            return this.http.get(`${APIConstant.GroupCodeList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
        }

         toggleActivate(id: number, isActive: boolean): Observable<any> {
    return this._baseService.put<any>(
      `${APIConstant.groupcode}/Activate/${id}/${isActive}`,
      {}
    );
  }
   // ✅ NEW: fetch edit data
  getByIdAsync(id: number) {
    return this._baseService.get<GroupCode>(`${APIConstant.groupcode}/GetByIdAsync?id=${id}`);
  }

  // ✅ NEW: post edit
  editGroupCode(model: GroupCode) {
    return this._baseService.post<number>(`${APIConstant.groupcode}/EditGroupCode`, model);
  }

}

