import { Routes } from '@angular/router';
import { GuestMasterListComponent } from './list/list.component';
import { GuestMasterAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: GuestMasterListComponent },
            { path: 'add', component: GuestMasterAddEditComponent },
            { path: 'edit/:id', component: GuestMasterAddEditComponent }
        ]
    }
] as Routes;
