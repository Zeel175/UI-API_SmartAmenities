import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { NgProgressModule } from '@ngx-progressbar/core';
import { NgProgressHttpModule } from '@ngx-progressbar/http';
import { NgProgressRouterModule } from '@ngx-progressbar/router';
import { AppResolver } from './app-resolver.service';
import { RouterModule } from '@angular/router';
import { appRoutes } from './app.routes';
import { AppComponent } from './app.component';
import { AppService } from './app.service';
import { PublicModule } from './public/public.module';
import { AgGridModule } from 'ag-grid-angular';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';
import { MatTableModule } from '@angular/material/table';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { NgxMatDatetimePickerModule, NgxMatTimepickerModule } from '@angular-material-components/datetime-picker';
import { LocationStrategy, HashLocationStrategy } from '@angular/common';
import { AuthInterceptor, CoreModule, ResponseInterceptor } from './core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // Required for Toastr
import { ToastrModule, ToastrService } from 'ngx-toastr'; // Import ToastrModule
import { AuthService } from './core/auth/auth.service';

export function initAuth(auth: AuthService) {
    // Return a function that returns a Promise. Angular will wait for it.
    return () => auth.check().toPromise();
}

@NgModule({
    declarations: [],
    imports: [
        BrowserAnimationsModule, // required by Toastr
        AppComponent,
        BrowserModule,
        CoreModule,
        PublicModule,
        RouterModule.forRoot(appRoutes, { scrollPositionRestoration: 'top' }),
        MatTableModule,
        MatFormFieldModule,
        NgxMatTimepickerModule,
        NgxMatDatetimePickerModule,
        MatInputModule,
        MatSelectModule,
        MatDatepickerModule,
        MatNativeDateModule,
        MatButtonModule,
        MatIconModule,
        AgGridModule,
        NgxMaterialTimepickerModule,
        ToastrModule.forRoot({
            timeOut: 3000,
            positionClass: 'toast-top-right',
            preventDuplicates: true
        }),
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: AuthInterceptor,
            multi: true
        },
        {
            provide: APP_INITIALIZER,
            useFactory: initAuth,
            deps: [AuthService],
            multi: true
        }
        // Uncomment below if you also use response interceptor
        // {
        //     provide: HTTP_INTERCEPTORS,
        //     useClass: ResponseInterceptor,
        //     multi: true
        // }
    ]
})
export class AppModule { }
