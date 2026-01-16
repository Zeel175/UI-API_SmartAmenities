import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
// import { UserAuthService } from 'app/core';
import { AuthService } from 'app/core/auth/auth.service';

@Directive({ selector: '[authPermission]' })
export class AuthPermissionDirective {

    constructor(private templateRef: TemplateRef<any>, private viewContainer: ViewContainerRef,
        //private authService: UserAuthService
         private authService: AuthService) {

    }

    @Input() set authPermission(permission: string) {
        // if (this.authService.hasPermission(permission) && this.templateRef) {
        //     this.viewContainer.createEmbeddedView(this.templateRef);
        // } else {
        //     this.viewContainer.clear();
        // }
        this.viewContainer.createEmbeddedView(this.templateRef);
    }
}
