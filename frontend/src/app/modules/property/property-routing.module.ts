import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApplicationPage, PageAuthGuard, PermissionType } from 'app/core';
// import { PropertyAddEditComponent } from './add-edit/add-edit.component';
// import { PropertyListComponent } from './list/list.component';
// import { PropertySearchPanelComponent } from './search-panel/search-panel.component';
import { PropertyComponent } from './property.component';
import { PropertyListComponent } from './list/list.component';
import { PropertyAddEditComponent } from './add-edit/add-edit.component';

// routes
const routes: Routes = [
    {
        path: '',
        component: PropertyComponent,
        children: [
            { path: '', redirectTo: 'list', pathMatch: 'full' },
            {
                path: 'list',
                //canActivate: [PageAuthGuard],
                component: PropertyListComponent,
                //data: { page: ApplicationPage.property, action: PermissionType.list, title: 'Property' }
            },
            {
                path: 'add',
                //canActivate: [PageAuthGuard],
               // data: { page: ApplicationPage.property, action: PermissionType.create, title: 'Property' },
                component: PropertyAddEditComponent
            },
            {
                path: 'edit/:id',
                //canActivate: [PageAuthGuard],
                //data: { page: ApplicationPage.property, action: PermissionType.edit, title: 'Property' },
                component: PropertyAddEditComponent
            }
        ]
    }
];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class PropertyRoutingModule { }

export const PropertyComponents = [
    PropertyComponent,
    PropertyListComponent, PropertyAddEditComponent,// PropertySearchPanelComponent
];