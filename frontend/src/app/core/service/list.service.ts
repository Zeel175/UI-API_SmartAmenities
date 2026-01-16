import { Injectable } from "@angular/core";
import { map, Observable } from "rxjs";
import { IListService } from "../interface";
import { BaseService } from "./base.service";
import { GroupCode, List, PermissionModule, Role, State } from "app/model";
import { APIConstant } from "../constant";
@Injectable({ providedIn: 'root' })

export class ListService implements IListService {

    constructor(private baseService: BaseService) {

    }

    getList(listName: string): Observable<List[]> {
        return this.baseService.get<List[]>(`${APIConstant.list[listName]}`).pipe(
            map((items: List[]) => {
                if (listName === 'currencies') {
                    const hasUSD = items.some(item => item.name?.toUpperCase() === 'USD');
                    if (!hasUSD) {
                        const maxId = items.reduce((max, item) => item.id && item.id > max ? item.id : max, 0);
                        items = [...items, { id: maxId + 1, name: 'USD' } as List];
                    }
                }
                return items;
            })
        );
    }

    getactiveroles(): Observable<Role[]> {
        return this.baseService.get<Role[]>(`${APIConstant.activeroles}`);
    }
 
    getAllStates(): Observable<State[]> {
        return this.baseService.get(`${APIConstant.state}`);
    }

     getStatesByCountryId(countryId: number): Observable<State[]> {
        return this.baseService.get(`${APIConstant.state}/GetStatesByCountryId/${countryId}`);
    }

    getCountries(): Observable<List[]> {
        return this.baseService.get<List[]>(`${APIConstant.country}`);
    }

    getModulePermissionList(): Observable<PermissionModule[]> {
        return this.baseService.get<PermissionModule[]>(`${APIConstant.list.modulePermission}`);
    }

}