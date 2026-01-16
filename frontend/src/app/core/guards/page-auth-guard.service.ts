// import { Injectable } from '@angular/core';
// import { Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
// import { UserAuthService } from '../service/auth.service';
// import { PermissionService } from '../service/permission.service';

// @Injectable()
// export class PageAuthGuard  {

//     constructor(private auth: UserAuthService, private router: Router) {

//     }



//    canActivate(route: ActivatedRouteSnapshot): boolean {
//       //  If the user does not any permission of feature
//         // if (!this.auth.hasPagePermission(route.data.page, route.data.action)) {
//         //     //Redirect to login page
//         //     this.router.navigate(['unauthorized']);
//         //     return false;
//         // }

//         return true;
//     }
// }

// src/app/core/guards/page-auth.guard.ts
import { Injectable } from '@angular/core';
import {
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
  UrlTree,
  Router
} from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';
import { PermissionService } from 'app/core/service/permission.service';
import { Observable, of, switchMap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class PageAuthGuard implements CanActivate {
  constructor(
    private auth: AuthService,
    private permission: PermissionService,
    private router: Router
  ) { }

  // canActivate(
  //   route: ActivatedRouteSnapshot,
  //   state: RouterStateSnapshot
  // ): boolean | UrlTree {
  //   // 1) Not logged in → force sign-in
  //   if (!this.auth.isLoggedIn()) {
  //     return this.router.createUrlTree(['/sign-in'], {
  //       queryParams: { redirectURL: state.url }
  //     });
  //   }

  //   // 2) Check page permission (route.data.page + route.data.action)
  //   const pageKey   = route.data?.['page'] as string;
  //   const actionKey = route.data?.['action'] as string;
  //   if (
  //     pageKey &&
  //     actionKey &&
  //     !this.permission.hasPagePermission(pageKey, actionKey)
  //   ) {
  //     return this.router.createUrlTree(['/unauthorized']);
  //   }


  //   return true;
  // }
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean | UrlTree> {
    
    // 1) Run our “check” which will rehydrate from token if needed
    return this.auth.check().pipe(
      switchMap(loggedIn => {
        if (!loggedIn) {
          // No valid session → redirect to sign-in
          return of(
            this.router.createUrlTree(['/sign-in'], {
              queryParams: { redirectURL: state.url }
            })
          );
        }
        // 2) Already authenticated → enforce page+action permission
        const page = route.data['page'] as string;
        const action = route.data['action'] as string;
        if (page && action && !this.permission.hasPagePermission(page, action)) {
          return of(this.router.createUrlTree(['/unauthorized']));
        }
        // 3) All good!
        return of(true);
      })
    );
  }


}
