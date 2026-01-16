import { Observable } from "rxjs";
import { List } from "app/model";

export interface IListService {

    getList: (listName: string) => Observable<List[]>;
}