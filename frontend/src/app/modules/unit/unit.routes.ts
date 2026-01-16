import { Routes } from '@angular/router';
import { UnitListComponent } from './list/list.component';
import { UnitAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: UnitListComponent },
            { path: 'add', component: UnitAddEditComponent },
            { path: 'edit/:id', component: UnitAddEditComponent }
        ]
    }
] as Routes;