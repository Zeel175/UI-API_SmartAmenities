import { Routes } from '@angular/router';
// import { UserListComponent } from './list/list.component';
// import { UserAddEditComponent } from './add-edit/add-edit.component';
import { ToastrService } from 'ngx-toastr';
import { PropertyListComponent } from './list/list.component';
import { PropertyAddEditComponent } from './add-edit/add-edit.component';
//import { RoleAddEditComponent } from './add-edit/add-edit.component';
//import { RoleListComponent } from './list/list.component';


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
            { path: '', component: PropertyListComponent },
            { path: 'add', component: PropertyAddEditComponent },
            { path: 'edit/:id', component: PropertyAddEditComponent }
        ]
    }
] as Routes;