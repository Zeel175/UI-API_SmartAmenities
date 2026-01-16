import { Routes } from '@angular/router';
// import { UserListComponent } from './list/list.component';
// import { UserAddEditComponent } from './add-edit/add-edit.component';
import { ToastrService } from 'ngx-toastr';
import { RoleAddEditComponent } from './add-edit/add-edit.component';
import { RoleListComponent } from './list/list.component';


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
            { path: '', component: RoleListComponent },
            { path: 'add', component: RoleAddEditComponent },
            { path: 'edit/:id', component: RoleAddEditComponent }
        ]
    }
] as Routes;