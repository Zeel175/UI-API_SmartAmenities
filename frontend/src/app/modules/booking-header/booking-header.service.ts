import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { APIConstant, BaseService, CRUDService } from 'app/core';
import { BookingHeader } from 'app/model';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class BookingHeaderService extends CRUDService<BookingHeader> {
    constructor(private _baseService: BaseService, private http: HttpClient) {
        super(_baseService, 'booking-header');
    }

    getBookings(pageIndex: number, pageSize: number) {
        return this.http.get(`${APIConstant.BookingHeaderListPaged}?pageIndex=${pageIndex}&pageSize=${pageSize}`);
    }

    getBookingById(id: number) {
        return this.http.get(`${APIConstant.BookingHeaderGetById}?id=${id}`);
    }

    addBooking(payload: BookingHeader) {
        return this.http.post(`${APIConstant.BookingHeaderAdd}`, payload);
    }

    updateBooking(payload: BookingHeader) {
        return this.http.post(`${APIConstant.BookingHeaderEdit}`, payload);
    }

    deleteBooking(id: number): Observable<void> {
        return this.http.delete<void>(`${APIConstant.BookingHeaderDelete}?id=${id}`);
    }

    getAmenities() {
        return this.http.get(`${APIConstant.AmenityMasterBasicList}`);
    }

    getAmenityUnitsByAmenityId(amenityId: number) {
        return this.http.get(`${APIConstant.AmenityUnitByAmenityId}?amenityId=${amenityId}`);
    }

    getSocieties() {
        return this.http.get(`${APIConstant.PropertyBasicList}`);
    }
}
