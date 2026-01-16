import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { APIConstant, BaseService, CRUDService } from "app/core";
import { List } from "app/model";
import { AuditLog } from "app/model/auditLog";
import { Observable } from "rxjs";
//import { AuditLog } from "src/app/model/auditLog";

@Injectable({ providedIn: 'root' })
export class AuditLogService extends CRUDService<AuditLog>{

    constructor(private _baseService: BaseService,private http: HttpClient) {
        super(_baseService, "auditLog");
    }
    getAuditLog(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.AuditLogList}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }
    // deleteCompany(id: number): Observable<any> {
    //     return this._baseService.delete(`${APIConstant.CompanyMasterList}/${id}`);
    // }
    
    // getCompanyById(id: number) {
    //     return this.http.get(`${APIConstant.CompanyMasterGetById}?id=${id}`);
    // }
    // addCompany(company: Company) {
    //     return this.http.post(`${APIConstant.CompanyMasterAdd}`, company);
    // }
    // updateCompany(company: Company) {
    //     return this.http.post(`${APIConstant.CompanyMasterEdit}`, company);
    // }
    
}
