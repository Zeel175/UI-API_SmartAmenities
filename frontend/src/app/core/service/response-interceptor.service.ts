import { Injectable } from "@angular/core";
import { HttpRequest, HttpInterceptor, HttpHandler, HttpEvent, HttpErrorResponse } from "@angular/common/http";
import { Observable } from "rxjs";
import { tap } from 'rxjs/operators';
import { Router } from "@angular/router";
import { NotificationService } from "./notification.service";

@Injectable()
export class ResponseInterceptor implements HttpInterceptor {

    constructor(private router: Router, private notificateService: NotificationService) { }

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        return next.handle(req).pipe(tap(evt => {
            if (evt instanceof HttpRequest) {

            }
        }, (err: any) => {
            if (err instanceof HttpErrorResponse) {
                if (err.status === 401) {
                    window.localStorage.clear();
                    this.router.navigate(['unauthorized']);
                }
                else if (err.status === 400 && err.error) {
                    var keys = Object.keys(err.error);
                    keys.forEach((key: string) => {
                        var errorMessage: string = err.error[key];

                        if (errorMessage.toString().toLowerCase().includes('invalid') && errorMessage.toString().toLowerCase().includes('token')) {
                            window.localStorage.clear();
                            this.router.navigate(['unauthorized']);
                            this.notificateService.error("Token Expired.");
                        }
                    });
                }
                else if (err.status === 500) {
                    this.notificateService.error("Something went wrong.", "Error");
                }
                else if (err.status === 0) {
                    if ((err.error.currentTarget.__zone_symbol__xhrURL).indexOf('localhost:8888') > -1) {
                        // not display any notificatin if no service is there.
                    }
                    else {
                        this.notificateService.error("Internet Issue.", "Error");
                    }
                }
            }
        }));
    }
}