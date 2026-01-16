import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RoleListComponent } from './list/list.component';
import { RoleAddEditComponent } from './add-edit/add-edit.component';
import { RoleComponent } from './role.component';
// import { UserListComponent } from './list/list.component';
// import { UserAddEditComponent } from './add-edit/add-edit.component';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
    },
    {
        path: 'list',
        // component: UserListComponent
        component: RoleListComponent
    },
    {
        path: 'add',
        //  component: UserAddEditComponent,
        component: RoleAddEditComponent
    },
    {
        path: 'edit/:id',
        // component: UserAddEditComponent
        component: RoleAddEditComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class RoleRoutingModule { }

export const RoleComponents = [
    RoleComponent, RoleListComponent, RoleAddEditComponent,
    // RoleSearchPanelComponent
]; 