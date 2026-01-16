// src/app/core/auth/auth.interceptor.ts
import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandlerFn,
  HttpRequest
} from '@angular/common/http';
import { inject }            from '@angular/core';
import { Observable, catchError, throwError } from 'rxjs';
import { AuthService }       from './auth.service';
import { AuthUtils }         from './auth.utils';

// helper: only try to decode real JWTs
function looksLikeJwt(token: string): boolean {
  return token.split('.').length === 3;
}

export const authInterceptor = (
  req: HttpRequest<unknown>,
  next: HttpHandlerFn
): Observable<HttpEvent<unknown>> => {
  const auth = inject(AuthService);
  const token = auth.accessToken;  // reads localStorage['accessToken']

  let newReq = req;
  if (token && looksLikeJwt(token)) {
    try {
      if (!AuthUtils.isTokenExpired(token)) {
        newReq = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
      }
    } catch {
      // malformed token → do nothing
    }
  }

  return next(newReq).pipe(
    catchError(err => {
      if (err instanceof HttpErrorResponse && err.status === 401) {
        auth.signOut().subscribe(() => location.reload());
      }
      return throwError(() => err);
    })
  );
};
