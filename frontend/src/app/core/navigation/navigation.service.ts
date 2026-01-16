// import { HttpClient } from '@angular/common/http';
// import { inject, Injectable } from '@angular/core';
// import { Navigation } from 'app/core/navigation/navigation.types';
// import { Observable, ReplaySubject, tap } from 'rxjs';

// @Injectable({ providedIn: 'root' })
// export class NavigationService {
//     private _httpClient = inject(HttpClient);
//     private _navigation: ReplaySubject<Navigation> =
//         new ReplaySubject<Navigation>(1);

//     // -----------------------------------------------------------------------------------------------------
//     // @ Accessors
//     // -----------------------------------------------------------------------------------------------------

//     /**
//      * Getter for navigation
//      */
//     get navigation$(): Observable<Navigation> {
//         return this._navigation.asObservable();
//     }

//     // -----------------------------------------------------------------------------------------------------
//     // @ Public methods
//     // -----------------------------------------------------------------------------------------------------

//     /**
//      * Get all navigation data
//      */
//     get(): Observable<Navigation> {
//         return this._httpClient.get<Navigation>('api/common/navigation').pipe(
//             tap((navigation) => {
//                 this._navigation.next(navigation);
//             })
//         );
//     }
// }


import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Navigation } from 'app/core/navigation/navigation.types';
import { Observable, ReplaySubject, tap } from 'rxjs';
import { PermissionService } from '../service/permission.service';

@Injectable({ providedIn: 'root' })
export class NavigationService {
    private _httpClient = inject(HttpClient);
    private _navigation: ReplaySubject<Navigation> = new ReplaySubject<Navigation>(1);
    private permissionService = inject(PermissionService);

    // -----------------------------------------------------------------------------------------------------
    // @ Accessors
    // -----------------------------------------------------------------------------------------------------

    /**
     * Getter for navigation
     */
    get navigation$(): Observable<Navigation> {
        return this._navigation.asObservable();
    }

    // -----------------------------------------------------------------------------------------------------
    // @ Public methods
    // -----------------------------------------------------------------------------------------------------

    /**
     * Get all navigation data with permission filtering
     */
    get(): Observable<Navigation> {
        return this._httpClient.get<Navigation>('api/common/navigation').pipe(
            tap((navigation) => {
                // Apply permission filtering
                const filteredNavigation = this.filterNavigationByPermissions(
                    navigation
                );
                this._navigation.next(filteredNavigation);
            })
        );
    }

    /**
     * Filter navigation items based on user permissions
     */
    private filterNavigationByPermissions(navigation: Navigation): Navigation {
        if (!navigation.default || !Array.isArray(navigation.default)) {
            return navigation;
        }

        // If no permissions are set, return navigation as is
        if (!this.permissionService.hasPermissions()) {
            return navigation;
        }

        const filteredDefault = navigation.default
            .map(section => this.filterSection(section))
            .filter(section => section !== null);

        return {
            ...navigation,
            default: filteredDefault,
            compact: filteredDefault,
            futuristic: filteredDefault,
            horizontal: filteredDefault
        };
    }

    /**
     * Filter a navigation section based on permissions
     */
    private filterSection(section: any): any {
        // Clone the section to avoid mutation
        const filteredSection = { ...section };

        if (filteredSection.children && Array.isArray(filteredSection.children)) {
            filteredSection.children = filteredSection.children
                .filter(item => this.hasPermissionForItem(item));
        }

        // Remove sections with no children
        if (filteredSection.children && filteredSection.children.length === 0) {
            return null;
        }

        return filteredSection;
    }

    /**
     * Check if user has permission for a specific navigation item
     */
    private hasPermissionForItem(item: any): boolean {
        if (!item.id) return true;

        // Always allow the "User" page without checking any permission:
        if (item.id === 'user') {
            return true;
        }
        // Map your route IDs to permission checks
        switch (item.id) {
            case 'user':
                return this.permissionService.hasPermissionPrefix('User (PER_USER)');
            case 'role':
                return this.permissionService.hasAnyPermissionForModule('Role (PER_ROLE)');
            case 'property':
                return this.permissionService.hasPermissionPrefix('Property (PER_PROPERTY)');
            case 'building':
                return this.permissionService.hasPermissionPrefix('Building (PER_BUILDING)');
            case 'floor':
                return this.permissionService.hasPermissionPrefix('Floor (PER_FLOOR)');
            case 'unit':
                return this.permissionService.hasPermissionPrefix('Unit (PER_UNIT)');
            case 'audit-log':
                return this.permissionService.hasPermissionPrefix('AuditLog (PER_AUDITLOG)');
            case 'group-code':
                return this.permissionService.hasPermissionPrefix('GroupCode (PER_GROUP_CODE)');
            case 'zone':
                return this.permissionService.hasPermissionPrefix('Zone (PER_ZONE)');
            case 'resident-master':
                return this.permissionService.hasPermissionPrefix('Resident (PER_RESIDENT)');
            case 'guest-master':
                return this.permissionService.hasPermissionPrefix('Guest (PER_GUEST)');
            case 'customer':
                return this.permissionService.hasPermissionPrefix('inv_customer_');
            case 'product':
                return this.permissionService.hasPermissionPrefix('inv_product');
            case 'permission':
                return this.permissionService.hasPermissionPrefix('adm_permission_');
            case 'module':
                return this.permissionService.hasPermissionPrefix('adm_module_');
            case 'module-group':
                return this.permissionService.hasPermissionPrefix('adm_modulegroup_');
            case 'location':
                return this.permissionService.hasPermissionPrefix('adm_location_');
            case 'terms':
                return this.permissionService.hasPermissionPrefix('inv_terms_');

            case 'process':
                return this.permissionService.hasPermissionPrefix('inv_process_');
            case 'vendor':
                return this.permissionService.hasPermissionPrefix('inv_vendor_');
            case 'machine':
                return this.permissionService.hasPermissionPrefix('inv_machine_');
            case 'product-group':
                return this.permissionService.hasPermissionPrefix('pr_group_');
            case 'inventory-type':
                return this.permissionService.hasPermissionPrefix('inv_inventorytypes_');
            case 'sales-order':
                return this.permissionService.hasPermissionPrefix('inv_salesorder_');
            // case 'stockledger':
            //     return this.permissionService.hasPermissionPrefix('inv_stockledger_');

            case 'productionWorkOrder':
                return this.permissionService.hasPermissionPrefix('pr_workorder_');
            case 'bomprocess':
                return this.permissionService.hasPermissionPrefix('pr_bomprocess_');
            case 'productionplanning':
                return this.permissionService.hasPermissionPrefix('pr_productionplanning');
            case 'monthlyProductionPlanning':
                return this.permissionService.hasPermissionPrefix('pr_monthlyproductionplanning_');
            case 'productionWorkDone':
                return this.permissionService.hasPermissionPrefix('pr_workdone_');
            case 'in-process-qa':
                return this.permissionService.hasPermissionPrefix('pr_inprocessqa_');
            case 'purchaseOrderPlanning':
                return this.permissionService.hasPermissionPrefix('pu_orderplanning');
            case 'purchase-inquiry':
                return this.permissionService.hasPermissionPrefix('pu_purchaseinquiry_');
            case 'purchaseorder':
                return this.permissionService.hasPermissionPrefix('pu_purchaseorder_');
            case 'purchaseplanning':
                return this.permissionService.hasPermission('pu_purchaseplanning_');
            case 'inward':
                return this.permissionService.hasPermissionPrefix('inv_inwards_');
            case 'outward':
                return this.permissionService.hasPermissionPrefix('inv_outwards_');
            case 'materialRequisition':
                return this.permissionService.hasPermissionPrefix('inv_materialrequisition_');
            // case 'designation':
            //     return this.permissionService.hasAnyPermissionForModule('Designation (PER_DESIGNATION)');
            // case 'projectmaster':
            //     return this.permissionService.hasAnyPermissionForModule('Project Master (PER_PROJECT_MASTER)');
            // case 'location':
            //     return this.permissionService.hasxAnyPermissionForModule('Location (PER_LOCATION)');
            // case 'service-category':
            //     return this.permissionService.hasAnyPermissionForModule('Service Category (PER_SERVICE_CATEGORY)');
            // case 'groupandlineitem':
            //     return this.permissionService.hasAnyPermissionForModule('Group Line Item (PER_GROUP_LINEITEM)');
            // case 'workflow':
            //     return this.permissionService.hasAnyPermissionForModule('Work Flow (PER_WORK_FLOW)');
            // case 'projectassignment':
            //     return this.permissionService.hasAnyPermissionForModule('Project Assignment (PER_PROJECT_ASSIGNMENT)');
            // case 'budget-entry-screen':
            //     return this.permissionService.hasAnyPermissionForModule('Budget Entry (PER_BUDGET_ENTRY)');
            // case 'actualinput':
            //     return this.permissionService.hasAnyPermissionForModule('Actual Input (PER_ACTUAL_INPUT)');
            // case 'provisionalinput':
            //     return this.permissionService.hasAnyPermissionForModule('Provisional Input (PER_PROVISIONAL_INPUT)');
            // case 'mtdreport':
            //     return this.permissionService.hasAnyPermissionForModule('MTD-Report (PRE_MTD_REPORT)');
            // case 'ytdreport':
            //     return this.permissionService.hasAnyPermissionForModule('YTD-Report (PRE_YTD_REPORT)');
            default:
                return true;
        }
    }
}
