import { Routes } from '@angular/router';
import { BookingHeaderListComponent } from './list/list.component';
import { BookingHeaderAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: BookingHeaderListComponent },
            { path: 'add', component: BookingHeaderAddEditComponent },
            { path: 'edit/:id', component: BookingHeaderAddEditComponent }
        ]
    }
] as Routes;
