import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { UserListComponent } from './list/list.component';
import { UserAddEditComponent } from './add-edit/add-edit.component';

const routes: Routes = [
    {
        path: '',
        redirectTo: 'list',
        pathMatch: 'full'
    },
    {
        path: 'list',
        component: UserListComponent
    },
    {
        path: 'add',
        component: UserAddEditComponent
    },
    {
        path: 'edit/:id',
        component: UserAddEditComponent
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class UserRoutingModule {}