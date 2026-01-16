import { Routes } from '@angular/router';
import { BuildingListComponent } from './list/list.component';
import { BuildingAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: BuildingListComponent },
            { path: 'add', component: BuildingAddEditComponent },
            { path: 'edit/:id', component: BuildingAddEditComponent }
        ]
    }
] as Routes;