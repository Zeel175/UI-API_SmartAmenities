import { Routes } from '@angular/router';
import { ZoneListComponent } from './list/list.component';
import { ZoneAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: ZoneListComponent },
            { path: 'add', component: ZoneAddEditComponent },
            { path: 'edit/:id', component: ZoneAddEditComponent }
        ]
    }
] as Routes;
