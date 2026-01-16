import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
// import { PermissionService } from '../service/permission.service';

@Injectable({
    providedIn: 'root'
})
export class PermissionGuard implements CanActivate {
    // constructor(private permissionService: PermissionService, private router: Router) {}

    canActivate(route: ActivatedRouteSnapshot): boolean {
        const requiredPermission = route.data['permission'];

        // Allow access if the user has the required permission
        // if (this.permissionService.hasPermission(requiredPermission)) {
        //     return true;
        // }

        // Extract the entity name from the permission string (e.g., "User (PER_USER)")
        const entity = requiredPermission.split(' - ')[0];

        // Allow access if the required permission is "View" and the user has "Add" permission for the same entity
        // if (requiredPermission.endsWith(' - View') &&
        //     this.permissionService.hasPermission(`${entity} - Add`)) {
        //     return true;
        // }

        // Redirect to unauthorized page if permission is not granted
        // this.router.navigate(['/unauthorized']);
        return false;
    }
}