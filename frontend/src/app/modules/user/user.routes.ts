import { Routes } from '@angular/router';
import { UserListComponent } from './list/list.component';
import { UserAddEditComponent } from './add-edit/add-edit.component';
import { ToastrService } from 'ngx-toastr';

export default [
    {

        path: '',
        providers: [

            {
                provide: 'ToastConfig',
                useValue: {
                    timeOut: 5000,
                    positionClass: 'toast-top-right',
                    preventDuplicates: true,
                }
            }
        ],
        children: [
            // { path: 'list', component: UserListComponent },

            { path: '', component: UserListComponent },
            { path: 'add', component: UserAddEditComponent },
            { path: 'edit/:id', component: UserAddEditComponent }
        ]
    }
] as Routes;