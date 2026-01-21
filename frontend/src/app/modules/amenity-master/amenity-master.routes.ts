import { Routes } from '@angular/router';
import { AmenityMasterListComponent } from './list/list.component';
import { AmenityMasterAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: AmenityMasterListComponent },
            { path: 'add', component: AmenityMasterAddEditComponent },
            { path: 'edit/:id', component: AmenityMasterAddEditComponent }
        ]
    }
] as Routes;
