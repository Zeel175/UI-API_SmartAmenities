// import { Injectable } from '@angular/core';
// import { Router, Route } from '@angular/router';
// import { UserAuthService } from '../service/auth.service';

// @Injectable()
// export class FeatureCanLoadGuard  {

//     constructor(private auth: UserAuthService, private router: Router) {

//     }

//     canLoad(route: Route): boolean {
//         // If the user does not any permission of feature
//         if (!this.auth.hasFeaturePermission(route.data.page)) {
//             //Redirect to login page
//             this.router.navigate(['unauthorized']);
//             return false;
//         }

//         return true;
//     }
// }
// src/app/core/guards/feature-can-load.guard.ts
import { Injectable }       from '@angular/core';
import { CanLoad, Route, UrlTree, Router } from '@angular/router';
import { AuthService }      from 'app/core/auth/auth.service';
import { PermissionService } from '../service/permission.service';


@Injectable({
  providedIn: 'root'
})
export class FeatureCanLoadGuard implements CanLoad {
  constructor(
    private auth: AuthService,
    private permission: PermissionService,
    private router: Router
  ) {}

  canLoad(route: Route): boolean | UrlTree {
    // 1) Must be logged in
    if (!this.auth.isLoggedIn()) {
      return this.router.createUrlTree(['/sign-in'], {
        queryParams: { redirectURL: route.path }
      });
    }

    // 2) Must have feature permission (route.data.page)
    const featureKey = route.data?.['page'] as string;
    if (featureKey && !this.permission.hasFeaturePermission(featureKey)) {
      return this.router.createUrlTree(['/unauthorized']);
    }

    return true;
  }
}
