// import { Injectable } from '@angular/core';
// import { ActivatedRouteSnapshot, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
// import { UserAuthService } from '../service/auth.service';

// @Injectable()
// export class AuthGuard  {

//     constructor(private auth: UserAuthService, private router: Router) { }

//     // canActivate(): boolean {
//     //     // If the user is not logged in we'll send them back to the home page

//     //     if (!this.auth.isLoggedIn()) {
//     //         //Redirect to login page
//     //         this.router.navigate(['unauthorized']);
//     //     }
//     //     return true;
//     // }
//     canActivate(
//     route: ActivatedRouteSnapshot,
//     state: RouterStateSnapshot
//   ): boolean | UrlTree {
//     debugger
//     if (this.auth.isLoggedIn()) {
//       return true;
//     }
//     // not logged in, redirect to unauthorized (or /login)
//     return this.router.createUrlTree(['/sign-in']);
//   }

// }
// src/app/core/auth/guards/auth.guard.ts
import { Injectable } from '@angular/core';
import {
    CanActivate,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    UrlTree,
    Router
} from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';  // ← use AuthService

@Injectable({
    providedIn: 'root'  // makes it available app-wide
})
export class AuthGuard implements CanActivate {

    constructor(
        private auth: AuthService,   // ← inject AuthService
        private router: Router
    ) { }

    canActivate(
        route: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ): boolean | UrlTree {
        if (this.auth.isLoggedIn()) {
            return true;
        }
        // not logged in → redirect to sign-in, preserving the requested URL
        return this.router.createUrlTree(['/sign-in'], {
            queryParams: { redirectURL: state.url }
        });
    }

}
