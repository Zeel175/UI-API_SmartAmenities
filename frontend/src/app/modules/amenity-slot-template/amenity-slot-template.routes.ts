import { Routes } from '@angular/router';
import { AmenitySlotTemplateListComponent } from './list/list.component';
import { AmenitySlotTemplateAddEditComponent } from './add-edit/add-edit.component';

export default [
    {
        path: '',
        children: [
            { path: '', component: AmenitySlotTemplateListComponent },
            { path: 'add', component: AmenitySlotTemplateAddEditComponent },
            { path: 'edit/:id', component: AmenitySlotTemplateAddEditComponent }
        ]
    }
] as Routes;
