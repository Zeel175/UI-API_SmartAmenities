import { Routes } from '@angular/router';
import { AmenityUnitMasterListComponent } from './list/list.component';
import { AmenityUnitMasterAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: AmenityUnitMasterListComponent },
            { path: 'add', component: AmenityUnitMasterAddEditComponent },
            { path: 'edit/:id', component: AmenityUnitMasterAddEditComponent }
        ]
    }
] as Routes;
