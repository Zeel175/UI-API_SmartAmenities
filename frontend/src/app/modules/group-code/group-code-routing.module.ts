import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GroupCodeListComponent } from './list/list.component';
import { GroupCodeAddEditComponent } from './add-edit/add-edit.component';
import { GroupCodeComponent } from './group-code.component';
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
        component: GroupCodeListComponent
    },
    {
        path: 'add',
        //  component: UserAddEditComponent,
        component: GroupCodeAddEditComponent
    },
    {
        path: 'edit/:id',
        // component: UserAddEditComponent
        component: GroupCodeAddEditComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class GroupCodeRoutingModule { }

export const GroupCodeComponents = [
    GroupCodeComponent, GroupCodeListComponent, GroupCodeAddEditComponent,
    // RoleSearchPanelComponent
]; 