// user.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { UserAddEditComponent } from './add-edit/add-edit.component';
import { UserService } from './user.service';
import { UserListComponent } from './list/list.component'; // Make sure this is imported
import { ToastrService } from 'ngx-toastr';
import { ToastrModule } from 'ngx-toastr';
import { UserRoutingModule } from './user-routing.module';
import { NgxMaskDirective } from 'ngx-mask';

@NgModule({
    imports: [
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
        UserAddEditComponent,
        UserListComponent,
        CommonModule,
        ReactiveFormsModule,
        FormsModule,
        UserRoutingModule,
        NgxMaskDirective

    ],
    providers: [
        UserService,
        // ToastrService // ✅ Keep this if you must, but ToastrService is already provided globally
    ]

})
export class UserModule { }
