import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { ResidentMaster } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ResidentMasterService extends CRUDService<ResidentMaster> {

  constructor(baseService: BaseService, private http: HttpClient) {
    super(baseService, 'resident-master');
  }

  getResidents(pageIndex: number, pageSize: number) {
    return this.http.get(`${APIConstant.ResidentMasterListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
  }

  getResidentById(id: number) {
    return this.http.get(`${APIConstant.ResidentMasterGetById}?id=${id}`);
  }

    addResident(payload: FormData) {
    return this.http.post(APIConstant.ResidentMasterAdd, payload);
  }

  updateResident(model: any, profilePhotoFile?: File, docs: File[] = []) {
  const fd = new FormData();

  fd.append('Id', String(model.id));
  fd.append('ParentFirstName', model.parentFirstName ?? '');
  fd.append('ParentLastName', model.parentLastName ?? '');
  fd.append('Email', model.email ?? '');
  fd.append('Mobile', model.mobile ?? '');
  fd.append('CountryCode', model.countryCode ?? '');
  fd.append('FaceId', model.faceId ?? '');
  fd.append('FingerId', model.fingerId ?? '');

  fd.append('IsResident', String(!!model.isResident));
  fd.append('IsActive', String(!!model.isActive));

  // UnitIds
  (model.unitIds || []).forEach((u: number, i: number) => {
    fd.append(`UnitIds[${i}]`, String(u));
  });

  // Profile photo
  if (profilePhotoFile) fd.append('ProfilePhotoFile', profilePhotoFile);

  // FamilyMembers
  (model.familyMembers || []).forEach((m: any, i: number) => {
    fd.append(`FamilyMembers[${i}].Id`, String(m.id || 0));
    fd.append(`FamilyMembers[${i}].FirstName`, m.firstName ?? '');
    fd.append(`FamilyMembers[${i}].LastName`, m.lastName ?? '');
    fd.append(`FamilyMembers[${i}].Email`, m.email ?? '');
    fd.append(`FamilyMembers[${i}].Mobile`, m.mobile ?? '');
    fd.append(`FamilyMembers[${i}].IsResident`, String(!!m.isResident));
    fd.append(`FamilyMembers[${i}].IsActive`, String(!!m.isActive));
    fd.append(`FamilyMembers[${i}].CardId`, m.cardId ?? '');
    fd.append(`FamilyMembers[${i}].QrId`, m.qrId ?? '');

    (m.unitIds || []).forEach((uid: number, j: number) => {
      fd.append(`FamilyMembers[${i}].UnitIds[${j}]`, String(uid));
    });

    if (m.profilePhotoFile) {
      fd.append(`FamilyMembers[${i}].ProfilePhotoFile`, m.profilePhotoFile);
    }
  });

  // Documents
  docs.forEach(f => fd.append('Documents', f));

  return this.http.post(`/api/ResidentMaster/Update`, fd);
}

  residentUpdate(payload: FormData) {
    return this.http.post(APIConstant.ResidentMasterEdit, payload);
  }


  deleteResident(id: number): Observable<void> {
    return this.http.delete<void>(`${APIConstant.ResidentMasterDelete}?id=${id}`);
  }
  getResidentDocuments(residentId: number) {
  return this.http.get<any[]>(
    `${APIConstant.ResidentMasterDocuments}?residentId=${residentId}`
  );
}

deleteResidentDocument(documentId: number) {
  return this.http.delete(
    `${APIConstant.ResidentMasterDocumentDelete}?documentId=${documentId}`
  );
}

getUsersByUnit(unitId: number) {
  return this.http.get(`${APIConstant.UsersByUnit}?unitId=${unitId}`);
}

}
