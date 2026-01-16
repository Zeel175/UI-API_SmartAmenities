import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
// import { APIConstant, BaseService, CRUDService } from "@app-core";
import { APIConstant, BaseService , CRUDService } from "app/core";
// import { List, User } from "@app-models";
import { List, User } from "app/model";
import { Observable, of } from "rxjs";
//import { BulkUploadResponse } from "src/app/model/BulkUploadResponse";

export interface EmailPayload { receptor: string; subject: string; body: string; }
export interface EmailTemplate { subject: string; body: string; receptor?: string; }

@Injectable({
    providedIn: 'root'  // This makes it available throughout the app
  })
export class EmailSenderService {
private readonly TEMPLATE_KEY = 'property-mail-template';
    constructor(private http: HttpClient) {   
    }
    sendEmail(emailData: { subject: string; receptor: string; body: string }): Observable<any> {
      //debugger
      return this.http.post(`${APIConstant.emailSend}`, emailData);
    }
    saveTemplate(tpl: EmailTemplate): Observable<void> {
    localStorage.setItem(this.TEMPLATE_KEY, JSON.stringify(tpl));
    return of(void 0);
  }

  getTemplate(): Observable<EmailTemplate | null> {
    const raw = localStorage.getItem(this.TEMPLATE_KEY);
    return of(raw ? JSON.parse(raw) : null);
  }
}


// <app-email-sender
//                         [userId]="selectedUserId"
//                         (emailSent)="handleEmailSent($event)"
//                       ></app-email-sender>