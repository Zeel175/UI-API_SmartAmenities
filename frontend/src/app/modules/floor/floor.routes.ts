import { Routes } from '@angular/router';
import { FloorListComponent } from './list/list.component';
import { FloorAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: FloorListComponent },
            { path: 'add', component: FloorAddEditComponent },
            { path: 'edit/:id', component: FloorAddEditComponent }
        ]
    }
] as Routes;