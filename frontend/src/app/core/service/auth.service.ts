// import { Injectable } from "@angular/core";
// import { CommonConstant } from "../constant";
// import { CommonUtility } from "../utilities";
// import { User, BrowserUser } from "app/model"; 

// @Injectable({
//   providedIn: 'root'
// })
// export class UserAuthService {

//     constructor() {
//     }

//     saveToken(token: string) {
//         window.localStorage.setItem(CommonConstant.token, token);
//        // console.log("token", window.localStorage.getItem(CommonConstant.token))
//     }

//     saveUser(user: BrowserUser) {
//         window.localStorage.setItem(CommonConstant.user, JSON.stringify(user));
//     }

//     getToken() {
//         return window.localStorage.getItem(CommonConstant.token);
//     }

//     getBrowserUser() {
//         let user = window.localStorage.getItem(CommonConstant.user);        
//         if (!CommonUtility.isEmpty(user)) {
//             return JSON.parse(user);
//         }
//         return null;
//     }

//     getUser() {
//         const user = this.getBrowserUser();
//         if (!CommonUtility.isEmpty(user) && !CommonUtility.isEmpty(user.user)) {
//             return {
//                 ...user.user,
//                 roles: user.user.roles || [] // Provide a fallback for roles
//             };
//         }
//         return null;
//     }
//     getPermission(): string[] {
//         const user = this.getBrowserUser();
//         if (!CommonUtility.isEmpty(user) && !CommonUtility.isEmpty(user.permissions)) {
//             return user.permissions;
//         }
//         return [];
//     }

//     hasFeaturePermission(feature: string): boolean {
//         return this.getPermission().some(x => x.startsWith(feature));
//     }

//     hasPagePermission(page: string, permission: string): boolean {
//         return this.hasPermission(`${page}_${permission}`);
//     }

//     hasPermission(permission: string): boolean {
//         return this.getPermission().some(x => x.toUpperCase() === permission.toUpperCase());
//     }

//     saveTheme(themeName: string) {
//         window.localStorage.setItem(CommonConstant.themeName, themeName);
//     }

//     getThemeName() {
//         return window.localStorage.getItem(CommonConstant.themeName);
//     }

//     isLoggedIn(): boolean {
//         return !CommonUtility.isNull(this.getToken());
//     }

//     loggedOut(): void {
//         window.localStorage.clear();
//     }
  
// }