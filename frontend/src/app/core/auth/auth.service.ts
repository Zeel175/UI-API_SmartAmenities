import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthUtils } from 'app/core/auth/auth.utils';
import { UserService } from 'app/core/user/user.service';
import { catchError, Observable, of, switchMap, throwError } from 'rxjs';
import { CommonConstant } from "../constant";
import { CommonUtility } from "../utilities";
// import { User } from 'app/model';
import { User, BrowserUser } from 'app/model/user';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
    authService: any;
    constructor(private router: Router) {

        const token = this.accessToken;
        if (token && !AuthUtils.isTokenExpired(token)) {
            this._authenticated = true;
            // (optional) fire-and-forget refresh of user from server
            this.signInUsingToken().subscribe();
        }
    }
    private _authenticated: boolean = false;
    private _httpClient = inject(HttpClient);
    private _userService = inject(UserService);

    set accessToken(token: string) {
        
        localStorage.setItem(CommonConstant.token, token);
    }
    get accessToken(): string {
        
        return localStorage.getItem(CommonConstant.token) ?? '';
    }
    // canActivate(): boolean {
    //     if (this.authService.isLoggedIn()) {
    //         return true;
    //     }

    //     this.router.navigate(['/login']);
    //     return false;
    // }

    canActivate(): boolean {
        
        if (this.isLoggedIn()) {
            return true;
        }
        this.router.navigate(['/login']);
        return false;
    }
    /**
     * Forgot password
     *
     * @param email
     */
    forgotPassword(email: string): Observable<any> {
        return this._httpClient.post('api/auth/forgot-password', email);
    }

    /**
     * Reset password
     *
     * @param password
     */
    resetPassword(password: string): Observable<any> {
        return this._httpClient.post('api/auth/reset-password', password);
    }

    /**
     * Sign in
     *
     * @param credentials
     */
    signIn(credentials: { email: string; password: string }): Observable<any> {
        // Throw error, if the user is already logged in
        if (this._authenticated) {
            return throwError('User is already logged in.');
        }

        return this._httpClient.post('api/auth/sign-in', credentials).pipe(
            switchMap((response: any) => {
                
                // Store the access token in the local storage
                this.accessToken = response.accessToken;

                // Set the authenticated flag to true
                this._authenticated = true;

                // Store the user on the user service
                this._userService.user = response.user;

                // Return a new observable with the response
                return of(response);
            })
        );
    }

    /**
     * Sign in using the access token
     */
    signInUsingToken(): Observable<any> {
        
        // Sign in using the token
        return this._httpClient
            .post('api/auth/sign-in-with-token', {
                accessToken: this.accessToken,
            })
            .pipe(
                catchError(() =>
                    // Return false
                    of(false)
                ),
                switchMap((response: any) => {

                    if (response.accessToken) {
                        this.accessToken = response.accessToken;
                    }

                    // Set the authenticated flag to true
                    this._authenticated = true;

                    // Store the user on the user service
                    this._userService.user = response.user;

                    // Return true
                    return of(true);
                })
            );
    }
    signOut(): Observable<any> {
        // Remove the access token from the local storage
        localStorage.removeItem('accessToken');
        localStorage.removeItem(CommonConstant.token);
        localStorage.removeItem('userId');
        localStorage.clear();
        // Set the authenticated flag to false
        this._authenticated = false;

        // Return the observable
        return of(true);
    }

    /**
     * Sign up
     *
     * @param user
     */
    signUp(user: {
        name: string;
        email: string;
        password: string;
        property: string;
    }): Observable<any> {
        return this._httpClient.post('api/auth/sign-up', user);
    }

    /**
     * Unlock session
     *
     * @param credentials
     */
    unlockSession(credentials: {
        email: string;
        password: string;
    }): Observable<any> {
        return this._httpClient.post('api/auth/unlock-session', credentials);
    }

    /**
     * Check the authentication status
     */
    check(): Observable<boolean> {
        
        // Check if the user is logged in
        if (this._authenticated) {
            return of(true);
        }

        // Check the access token availability
        if (!this.accessToken) {
            return of(false);
        }

        // Check the access token expire date
        if (AuthUtils.isTokenExpired(this.accessToken)) {
            return of(false);
        }

        // If the access token exists, and it didn't expire, sign in using it
        return this.signInUsingToken();
    }
    saveToken(token: string) {
        window.localStorage.setItem(CommonConstant.token, token);
        this.accessToken = token;
    }

    saveUser(user: BrowserUser) {
        window.localStorage.setItem(CommonConstant.user, JSON.stringify(user));
    }

    getToken() {
        return window.localStorage.getItem(CommonConstant.token);
    }

    getBrowserUser() {
        let user = window.localStorage.getItem(CommonConstant.user);
        if (!CommonUtility.isEmpty(user)) {
            return JSON.parse(user);
        }
        return null;
    }

    getUser() {
        const user = this.getBrowserUser();
        if (!CommonUtility.isEmpty(user) && !CommonUtility.isEmpty(user.user)) {
            return {
                ...user.user,
                roles: user.user.roles || [] // Provide a fallback for roles
            };
        }
        return null;
    }
    getPermission(): string[] {
        const user = this.getBrowserUser();
        if (!CommonUtility.isEmpty(user) && !CommonUtility.isEmpty(user.permissions)) {
            return user.permissions;
        }
        return [];
    }

    hasFeaturePermission(feature: string): boolean {
        return this.getPermission().some(x => x.startsWith(feature));
    }

    hasPagePermission(page: string, permission: string): boolean {
        return this.hasPermission(`${page}_${permission}`);
    }

    hasPermission(permission: string): boolean {
        return this.getPermission().some(x => x.toUpperCase() === permission.toUpperCase());
    }

    saveTheme(themeName: string) {
        window.localStorage.setItem(CommonConstant.themeName, themeName);
    }

    getThemeName() {
        return window.localStorage.getItem(CommonConstant.themeName);
    }

    // isLoggedIn(): boolean {
    //     return !CommonUtility.isNull(this.getToken());
    // }
    isLoggedIn(): boolean {
        const token = this.accessToken;
        return !!token && !AuthUtils.isTokenExpired(token);
    }


    loggedOut(): void {
        window.localStorage.clear();
    }

    markAuthenticated(): void {
        this._authenticated = true;
    }
}
