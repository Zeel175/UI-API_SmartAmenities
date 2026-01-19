import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { GuestMaster } from 'app/model';
import { environment } from 'environments/environment';
import { Observable } from 'rxjs';

export interface GuestCreateRequest {
  firstName: string;
  lastName: string;
  email?: string;
  mobile?: string;
  unitId: number;
  cardId?: string;
  qrId?: string;
  isActive: boolean;
}

export interface GuestUpdateRequest {
  id: number;
  firstName: string;
  lastName: string;
  email?: string;
  mobile?: string;
  unitId: number;
  cardId?: string;
  qrId?: string;
  isActive: boolean;
}

@Injectable({ providedIn: 'root' })
export class GuestMasterService extends CRUDService<GuestMaster> {
 private readonly baseUrl = environment.serverPath 
  constructor(baseService: BaseService, private http: HttpClient) {
    super(baseService, 'guest-master');
  }

  getGuests(pageIndex: number, pageSize: number) {
    return this.http.get(`${APIConstant.GuestMasterListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
  }

  getGuestById(id: number) {
    return this.http.get(`${APIConstant.GuestMasterGetById}?id=${id}`);
  }

  addGuest(payload: FormData) {
    return this.http.post(APIConstant.GuestMasterAdd, payload);
  }

  updateGuest(payload: GuestUpdateRequest) {
    return this.http.post(APIConstant.GuestMasterEdit, payload);
  }


  deleteGuest(id: number): Observable<void> {
    return this.http.delete<void>(`${APIConstant.GuestMasterDelete}?id=${id}`);
  }
  getGuestsByUnit(unitId: number) {
    return this.http.get(`${APIConstant.GuestMasterByUnit}?unitId=${unitId}`);
  }
  getGuestDocuments(guestId: number) {
  return this.http.get<any[]>(
    `${APIConstant.GuestMasterDocuments}?guestId=${guestId}`
  );
}

deleteGuestDocument(documentId: number) {
  return this.http.delete(
    `${APIConstant.GuestMasterDocumentDelete}?documentId=${documentId}`
  );
}
createGuest(payload: GuestCreateRequest[]): Observable<any> {
    return this.http.post(
      `${this.baseUrl}api/GuestMaster/CreateGuest`,
      payload
    );
  }
}
