import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";
//  import { APIConstant, BaseService, CRUDService } from "@app-services";
import { APIConstant, BaseService, CRUDService } from "app/core";
//  import { List, User } from "@app-models";
import { List, User } from "app/model";
import { PaginatedList } from "app/model/PaginatedList";
import { UserProjectDetails } from "app/model/userProjectDetails";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'  // Add this back
})

export class UserService extends CRUDService<User> {

   constructor(private _baseService: BaseService) {
        super(_baseService, "user");
    }

    getReportsToUsers(townId: number, excludeUserId: number): Observable<List[]> {
        return this._baseService.get(`${APIConstant.account}/gettownusers/${townId}/${excludeUserId}`);
    }
     // Method to get paginated user data
     getUsers(pageIndex: number, pageSize: number): Observable<any> {
        return this._baseService.get(`${APIConstant.user}/paged?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }
    assignRole(userId: number, role: string): Observable<any> {
        return this._baseService.post(`${APIConstant.user}/${userId}/assign-role?role=${role}`, {});
    }
    removeRole(userId: number, role: string): Observable<any> {
        return this._baseService.post(`${APIConstant.user}/${userId}/remove-role?role=${role}`, {});
    }
    getRoles(userId: number): Observable<string[]> {
        return this._baseService.get(`${APIConstant.user}/${userId}/roles`);
    }
    deleteUser(id: number): Observable<any> {
        return this._baseService.delete(`${APIConstant.UserList}/${id}`);
    }
    
}