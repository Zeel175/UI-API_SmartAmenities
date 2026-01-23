import { Route } from '@angular/router';
import { initialDataResolver } from 'app/app.resolvers';
import { AuthGuard } from 'app/core/auth/guards/auth.guard';
import { NoAuthGuard } from 'app/core/auth/guards/noAuth.guard';
import { LayoutComponent } from 'app/layout/layout.component';
import { LoginComponent } from './public/login/login.component';


export const appRoutes: Route[] = [

    { path: '', pathMatch: 'full', redirectTo: 'example' },
    { path: 'signed-in-redirect', pathMatch: 'full', redirectTo: 'example' },
    // { path: 'login-redirect', pathMatch: 'full', redirectTo: 'login' },
    // { path: '', redirectTo: 'login', pathMatch: 'full' },
    // { path: 'login', component: LoginComponent },
    // { path: 'user', loadChildren: () => import('app/modules/user/user.routes') },
    {
        path: '',
        canActivate: [NoAuthGuard],
        canActivateChild: [NoAuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            { path: 'confirmation-required', loadChildren: () => import('app/modules/auth/confirmation-required/confirmation-required.routes') },
            { path: 'forgot-password', loadChildren: () => import('app/modules/auth/forgot-password/forgot-password.routes') },
            { path: 'reset-password', loadChildren: () => import('app/modules/auth/reset-password/reset-password.routes') },
            { path: 'sign-in', loadChildren: () => import('app/modules/auth/sign-in/sign-in.routes') },
            { path: 'sign-up', loadChildren: () => import('app/modules/auth/sign-up/sign-up.routes') },
            // { path: 'user', loadChildren: () => import('app/modules/user/user.routes') },
        ]

    },
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            { path: 'sign-out', loadChildren: () => import('app/modules/auth/sign-out/sign-out.routes') },
            { path: 'unlock-session', loadChildren: () => import('app/modules/auth/unlock-session/unlock-session.routes') }

        ]
    },
    {
        path: '',
        component: LayoutComponent,
        data: {
            layout: 'empty'
        },
        children: [
            { path: 'home', loadChildren: () => import('app/modules/landing/home/home.routes') },
        ]
    },
    {
        path: '',
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        component: LayoutComponent,
        resolve: {
            initialData: initialDataResolver
        },
        children: [
            { path: 'example', loadChildren: () => import('app/modules/admin/example/example.routes') },
            { path: 'user', loadChildren: () => import('app/modules/user/user.routes') },
            { path: 'role', loadChildren: () => import('app/modules/role/role.routes') },
            { path: 'property', loadChildren: () => import('app/modules/property/property.routes') },
            { path: 'building', loadChildren: () => import('app/modules/building/building.routes') },
            { path: 'device', loadChildren: () => import('app/modules/device/device.routes') },
            { path: 'floor', loadChildren: () => import('app/modules/floor/floor.routes') },
            { path: 'unit', loadChildren: () => import('app/modules/unit/unit.routes') },
            { path: 'resident-master', loadChildren: () => import('app/modules/resident-master/resident-master.routes') },
            { path: 'guest-master', loadChildren: () => import('app/modules/guest-master/guest-master.routes') },
            { path: 'amenity-master', loadChildren: () => import('app/modules/amenity-master/amenity-master.routes') },
            { path: 'amenity-unit-master', loadChildren: () => import('app/modules/amenity-unit-master/amenity-unit-master.routes') },
            { path: 'amenity-slot-template', loadChildren: () => import('app/modules/amenity-slot-template/amenity-slot-template.routes') },
            { path: 'booking-header', loadChildren: () => import('app/modules/booking-header/booking-header.routes') },
            { path: 'audit-log', loadChildren: () => import('app/modules/audit-log/audit-log.routes') },
            { path: 'hikvision-logs', loadChildren: () => import('app/modules/hikvision-logs/hikvision-logs.routes') },
            { path: 'group-code', loadChildren: () => import('app/modules/group-code/group-code.routes') },
            { path: 'zone', loadChildren: () => import('app/modules/zone/zone.routes') },
        ]
    }
];
