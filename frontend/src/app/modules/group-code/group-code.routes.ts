// group-code.routes.ts
import { Routes } from '@angular/router';
import { GroupCodeListComponent } from './list/list.component';      // Standalone!
import { GroupCodeAddEditComponent } from './add-edit/add-edit.component';  // Standalone!

export default [
    {
        path: '',
        children: [
            { path: '', component: GroupCodeListComponent },
            { path: 'list', component: GroupCodeListComponent },
            { path: 'add', component: GroupCodeAddEditComponent },
            { path: 'edit/:id', component: GroupCodeAddEditComponent }
        ]
    }
] as Routes;
