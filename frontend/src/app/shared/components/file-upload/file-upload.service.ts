import { HttpClient, HttpErrorResponse, HttpEventType, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
// import { APIConstant } from '@app-core';
import { APIConstant } from 'app/core';
import { catchError, map, Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {

  constructor(private http: HttpClient) { }

  uploadFile(fileData: FormData): Observable<any> {
    return this.http.post<any>(`${APIConstant.uploadFile}`, fileData);
  }

  // uploadFile(fileData: FormData): Observable<number> {
  //   const uploadUrl = 'your-api-endpoint'; // Replace with your actual API endpoint

  //   return this.http.post(uploadUrl, fileData, {
  //     headers: new HttpHeaders(),
  //     observe: 'events',
  //     reportProgress: true
  //   }).pipe(
  //     map(event => {
  //       switch (event.type) {
  //         case HttpEventType.UploadProgress:
  //           if (event.total) {
  //             return Math.round((100 * event.loaded) / event.total); // Return progress percentage
  //           }
  //           break;
  //         case HttpEventType.Response:
  //           return 100; // Upload complete
  //       }
  //       return 0;
  //     }),
  //    // catchError(error => this.handleError(error)) // Pass the error to handleError
  //   );
  // }

  // private handleError(error: HttpErrorResponse) {
  //   console.error('File upload error: ', error);
  //   throw error;
  // }
}
