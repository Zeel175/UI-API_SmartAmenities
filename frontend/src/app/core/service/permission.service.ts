import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {

  private permissions: string[] = [];

  constructor() {
    this.setPermissions();
  }

  // Check if any permissions are loaded
  hasPermissions(): boolean {
    return this.permissions && this.permissions.length > 0;
  }

  // Set permissions, typically called after fetching from an API
  setPermissions(): void {

    this.permissions = JSON.parse(localStorage.getItem('userPermissions'));
    // this.permissions =  permissions;
    // console.log("permission from service", this.permissions);
  }

  // Check if user has the required permission
  hasPermission(permission: string): boolean {
    return this.permissions.includes(permission);
  }
  // permission.service.ts
  hasAnyPermissionForModule(moduleName: string): boolean {
    if (!this.permissions || this.permissions.length === 0) return false;

    return this.permissions.some(p => p.startsWith(moduleName));
  }

  hasPermissionPrefix(prefix: string): boolean {
    return this.permissions && this.permissions.some(p => p.startsWith(prefix));
  }
  hasPagePermission(page: string, action: string): boolean {
    const key = `${page}_${action}`.toLowerCase();
    // Compare case-insensitive if your permissions are lowercase:
    return this.permissions
      .map(p => p.toLowerCase())
      .includes(key);
  }
  hasFeaturePermission(feature: string): boolean {
    // reuse your existing helper
    return this.hasAnyPermissionForModule(feature);
  }
}
