import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UntypedFormBuilder, FormControl, UntypedFormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
// import { UserAuthService, APIConstant, CommonUtility } from '@app-core';
import { APIConstant, CommonUtility } from 'app/core';
import { PublicService } from '../public.service';
// import { CookieService } from 'ngx-cookie-service';
import { CookieService } from 'ngx-cookie-service';
import { PermissionService } from 'app/core/service/permission.service';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from 'app/core/auth/auth.service';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';

// @Component({
//     templateUrl: './login.component.html',
//     styleUrls: ['login.component.scss']
// })
@Component({
    selector: 'app-login', // <-- should be a tag name
    templateUrl: './login.component.html', // <-- should be HTML
    styleUrls: ['./login.component.scss'], // <-- styles go here
    standalone: true,
    imports: [
        ReactiveFormsModule,
        FormsModule,
        RouterModule,
        MatFormFieldModule,
        MatCardModule,
        MatInputModule,
        MatSelectModule,
        MatIconModule,
        MatButtonModule,
        CommonModule,
        MatCheckboxModule
        // ToastrModule
    ]
})
// ...existing code...
export class LoginComponent implements OnInit {
    frmLogin: UntypedFormGroup = new UntypedFormGroup({});
    isFormSubmitted: boolean = false;
    cookieUserName = '';
    cookiePassword = '';
    hidePassword: boolean = true;
    error: any;
    password: string = "";
    userName: string = "";
    ssoToken: string = '';
    loading: boolean = false;
    route: any;

    constructor(private router: Router, private fb: UntypedFormBuilder, private activatedRoute: ActivatedRoute,
        private publicService: PublicService, //private userAuthService: UserAuthService,
        
        private permissionService: PermissionService, private cookieService: CookieService,
        private notificationService: ToastrService, private authService: AuthService) {
        this.createForm();

        const cookieExists: boolean = cookieService.check('Skyward_UserName');
        if (cookieExists) {
            this.cookieUserName = this.cookieService.get('Skyward_UserName');
            this.cookiePassword = this.cookieService.get('Skyward_Password');
            this.frmLogin.controls.username.setValue(this.cookieUserName);
            this.frmLogin.controls.password.setValue(this.cookiePassword);
            this.frmLogin.controls.rememberMe.setValue(true);
        }
    }

    ngOnInit(): void {
        this.getLoginRoute();
    }

    private getLoginRoute() {
        this.activatedRoute.params.subscribe((params) => {

        });
    }

    private createForm() {
        this.frmLogin = this.fb.group({
            userName: ['', [Validators.required]],
            password: ['', [Validators.required]],
            rememberMe: [false]
        });
    }

    login() {
        this.isFormSubmitted = true;
        if (!this.frmLogin.valid) {
            return;
        }
        this.error = null;
        this.loading = true;

        const loginData = this.frmLogin.value;
        this.password = loginData.password;
        this.userName = loginData.userName;
        this.publicService.login(loginData)
            .subscribe((result) => {
                this.loading = false;

                if (result.isSuccess == true) {
                  //  this.userAuthService.saveToken(result.token);
                   // this.userAuthService.saveUser(result);
                    this.authService.saveToken(result.token);
                    this.authService.saveUser(result);
                     if (result.user && result.user.id) {
                        localStorage.setItem('userId', result.user.id.toString());
                    }

                    setTimeout(() => {
                        const userPermissions = result.permissions;
                        localStorage.setItem('userPermissions', JSON.stringify(userPermissions));
                        const name = result?.user?.name || result?.user?.userName || result?.displayName || result?.name || '';
                        localStorage.setItem('Name', name);
                        //  console.log("Local stroage name", localStorage.getItem('Name'));
                        this.permissionService.setPermissions();
                        // const redirectURL = this.activatedRoute.snapshot.queryParams['redirectURL'] || '/user';
                        // this.router.navigateByUrl(redirectURL);
                        const redirectURL =
                            this.route.snapshot.queryParamMap.get('redirectURL') || '/user/list';
                        this.router.navigateByUrl(redirectURL);


                    }, 0);

                    if (loginData.rememberMe === true) {
                        this.cookieService.set('Skyward_User', loginData.userName);
                    }
                    else {
                        this.cookieService.delete('Skyward_User');
                    }
                }
                else {

                    this.error = { error: [result.message] }
                }
            }, (error) => {
                this.loading = false;
                if (error && error.status === 400) {
                    this.error = error.error ? (error.error.modelState || null) : null;
                    console.log(this.error);
                }
                else if (error && error.status === 401) {
                    console.log("issuccess false");
                    this.notificationService.warning("Invalid Username or Password");
                    this.router.navigateByUrl('login');
                }
                else if (error && error.status === 500) {
                    this.error = { error: ["Something went wrong. Please try again later."] };
                }
                else {
                    this.error = { error: ["Please check your internet connection."] };
                }
            });
    }
}