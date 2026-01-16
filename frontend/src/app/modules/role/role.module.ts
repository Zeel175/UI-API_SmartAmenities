import { NgModule } from '@angular/core';
import { SharedModule } from 'app/shared';
import { CommonModule } from '@angular/common';
// import { RoleComponents, RoleRoutingModule } from './role-routing.module';
import { RoleComponent } from './role.component';
import { RoleRoutingModule } from './role-routing.module';
import { RoleService } from './role.service';
import { provideEnvironmentNgxMask } from 'ngx-mask';
import { NgxDatatableModule } from '@swimlane/ngx-datatable';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { UserAddEditComponent } from '../user/add-edit/add-edit.component';
import { UserListComponent } from '../user/list/list.component';
import { UserRoutingModule } from '../user/user-routing.module';
import { MatCheckboxModule } from '@angular/material/checkbox';
@NgModule({
    // declarations: [
    //     [...RoleComponent]
    // ],
    imports: [
        SharedModule,
        RoleRoutingModule, CommonModule, NgxDatatableModule,
        MatPaginatorModule,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        // ToastrModule,
        MatFormFieldModule,
        MatInputModule,
        MatSelectModule,
        MatButtonModule,
        MatCardModule,
        MatIconModule,
        RouterModule,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        UserRoutingModule, MatCheckboxModule

    ],

    providers: [
        RoleService, provideEnvironmentNgxMask()
    ]
})
export class RoleModule { }