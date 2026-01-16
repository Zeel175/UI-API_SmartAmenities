import { Routes } from '@angular/router';
import { ResidentMasterListComponent } from './list/list.component';
import { ResidentMasterAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: ResidentMasterListComponent },
            { path: 'add', component: ResidentMasterAddEditComponent },
            { path: 'edit/:id', component: ResidentMasterAddEditComponent }
        ]
    }
] as Routes;
