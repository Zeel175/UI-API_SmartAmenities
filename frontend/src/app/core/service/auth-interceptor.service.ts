// import { Injectable } from "@angular/core";
// import { HttpRequest, HttpInterceptor, HttpHandler, HttpEvent } from "@angular/common/http";
// import { CommonConstant, PublicAPI } from "../constant";
// import { Observable } from "rxjs";
// import { UserAuthService } from "app/core/service/auth.service"; // <-- Update path as per your app

// @Injectable()
// export class AuthInterceptor implements HttpInterceptor {

//     constructor(
//         private userAuthService: UserAuthService
//     ) {}

//     intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
//         // Skip auth for public APIs
//         if (PublicAPI.some(x => req.url.indexOf(x) > -1)) {
//             return next.handle(req);
//         }

//         // Use service to get token!
//         const token = this.userAuthService.getToken();
//         let newReq = req.clone({
//             headers: req.headers.set('Authorization', `Bearer ${token}`)
//         });

//         // Report API: expect blob
//         if (newReq.url.indexOf("api/report") > -1) {
//             newReq = newReq.clone({ responseType: 'blob' as 'json' });
//         } else {
//             newReq = newReq.clone({
//                 headers: newReq.headers.set('Accept', 'application/json')
//             });
//         }

//         return next.handle(newReq);
//     }
// }
// src/app/core/auth/auth.interceptor.ts
import { Injectable } from "@angular/core";
import {
  HttpRequest,
  HttpInterceptor,
  HttpHandler,
  HttpEvent
} from "@angular/common/http";
import { Observable } from "rxjs";
import { CommonConstant, PublicAPI } from "../constant";
import { AuthService } from "../auth/auth.service";


@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(
    private auth: AuthService               // ← inject AuthService, not UserAuthService
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // 1) Skip any public endpoints
    if (PublicAPI.some(url => req.url.includes(url))) {
      return next.handle(req);
    }

    // 2) Grab the one token from AuthService
    const token = this.auth.accessToken;

    // 3) Build a single cloned request with both headers at once
    let headers = req.headers;
    if (token) {
      headers = headers.set("Authorization", `Bearer ${token}`);
    }
    headers = headers.set("Accept", "application/json");

    // 4) If it's a report download, force blob response
    const opts: any = { headers };
    if (req.url.includes("/api/report")) {
      opts.responseType = "blob";
    }

    const newReq = req.clone(opts);

    // 5) Send it on its way
    return next.handle(newReq);
  }
}
