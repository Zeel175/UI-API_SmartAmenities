import { Routes } from '@angular/router';
import { AuditLog } from 'app/model/auditLog';
// import { UserListComponent } from './list/list.component';
// import { UserAddEditComponent } from './add-edit/add-edit.component';
import { ToastrService } from 'ngx-toastr';
import { AuditLogListComponent } from './list/list.component';



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
            { path: '', component: AuditLogListComponent },
            //{ path: 'add', component: CompanyAddEditComponent },
           // { path: 'edit/:id', component: CompanyAddEditComponent }
        ]
    }
] as Routes;