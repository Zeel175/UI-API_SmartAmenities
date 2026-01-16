import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { BaseService, APIConstant } from "app/core";


@Injectable({
    providedIn: 'root'  // Add this back
})
export class PublicService {

    constructor(private baseService: BaseService) {

    }

    login(data: any): Observable<any> {
        return this.baseService.post(`${APIConstant.login}`, data);
    }
}