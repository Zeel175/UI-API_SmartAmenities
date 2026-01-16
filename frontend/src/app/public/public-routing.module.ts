import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { PublicComponent } from './public.component';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { LogoutComponent } from './logout/logout.component';
import { AppResolver } from 'app/app-resolver.service';


//routes
const routes: Routes = [
    {
        path: '',
        component: PublicComponent,
        //resolve: { 'themeData': AppResolver },
        children: [
            { path: '', redirectTo: 'login', pathMatch: 'full' },
            { path: 'login', component: LoginComponent, data: { title: 'Login' } },
            { path: 'login/:ssotoken', component: LoginComponent, data: { title: 'Login' } },
            { path: 'unauthorized', component: UnauthorizedComponent, data: { title: 'Unauthorized' } },
            { path: 'logout', component: LogoutComponent, data: { title: 'Logout' } }
                       
        ]
    }
];

@NgModule({
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [RouterModule],
    providers: [AppResolver],
})
export class PublicRoutingModule { }

export const PublicComponents = [
    PublicComponent, LoginComponent, UnauthorizedComponent, LogoutComponent
];