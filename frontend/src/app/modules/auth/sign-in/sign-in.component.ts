import { Component, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import {
    FormsModule,
    NgForm,
    ReactiveFormsModule,
    UntypedFormBuilder,
    UntypedFormGroup,
    Validators,
} from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { fuseAnimations } from '@fuse/animations';
import { FuseAlertComponent, FuseAlertType } from '@fuse/components/alert';
// import { UserAuthService } from 'app/core';
import { AuthService } from 'app/core/auth/auth.service';
import { PermissionService } from 'app/core/service/permission.service';
import { PublicService } from 'app/public/public.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatValidationMessage } from 'app/shared/components/validation-message/mat-validation-message';
import { ToastrService } from 'ngx-toastr';
import { UserService } from 'app/core/user/user.service';
@Component({
    selector: 'auth-sign-in',
    templateUrl: './sign-in.component.html',
    encapsulation: ViewEncapsulation.None,
    animations: fuseAnimations,
    imports: [
        RouterLink,
        FuseAlertComponent,
        FormsModule,
        ReactiveFormsModule,
        MatFormFieldModule,
        MatInputModule,
        MatButtonModule,
        MatIconModule,
        MatCheckboxModule,
        MatProgressSpinnerModule
    ],
})
export class AuthSignInComponent implements OnInit {
    @ViewChild('signInNgForm') signInNgForm: NgForm;

    alert: { type: FuseAlertType; message: string } = {
        type: 'success',
        message: '',
    };
    signInForm: UntypedFormGroup;
    showAlert: boolean = false;

    /**
     * Constructor
     */
    constructor(
        private _activatedRoute: ActivatedRoute,
        private _formBuilder: UntypedFormBuilder,
        private _authService: AuthService,
        private _router: Router,
        private publicService: PublicService,
        // private userAuthService: UserAuthService,
        private permissionService: PermissionService,
        private _snackBar: MatSnackBar,
        private notificationService: ToastrService,
        private userService: UserService
    ) { }

    // -----------------------------------------------------------------------------------------------------
    // @ Lifecycle hooks
    // -----------------------------------------------------------------------------------------------------

    /**
     * On init
     */
    ngOnInit(): void {
        // Create the form
        this.signInForm = this._formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required],
            rememberMe: [false]
        });

    }

    signIn(): void {
        if (this.signInForm.invalid) {
            // Show toastr if username or password field is empty or invalid
            this.notificationService.error('Please enter a valid username and password', 'Login Failed', {
                timeOut: 3000,
                positionClass: 'toast-top-right'
            });
            return;
        }
        this.signInForm.disable();
        this.showAlert = false;

        const loginData = {
            username: this.signInForm.value.username,
            password: this.signInForm.value.password
        };

        this.publicService.login(loginData).subscribe({
            next: (result) => {
                if (result.isSuccess) {
                    this.userService.user = result.user;
                    // Save session info

                    // this.userAuthService.saveToken(result.token);
                    //  this.userAuthService.saveUser(result);
                    this._authService.saveToken(result.token);
                    this._authService.accessToken = result.token;
                    this._authService.saveUser(result);
                    this._authService.markAuthenticated();
                    // after you get back `result.user`
                    //  this.userService.setUser(result.user);
                    

                    if (result.user && result.user.id) {
                        localStorage.setItem('userId', result.user.id.toString());
                    }
                    localStorage.setItem('userPermissions', JSON.stringify(result.permissions));
                    const name = result?.user?.name || result?.user?.userName || result?.displayName || result?.name || '';
                    localStorage.setItem('Name', name);
                    this.permissionService.setPermissions();

                    this.signInForm.enable();

                    const redirectURL =
                        this._activatedRoute.snapshot.queryParamMap.get('redirectURL') || '/signed-in-redirect';

                    this._router.navigateByUrl(redirectURL);
                } else {
                    this.handleLoginError('Login failed: ' + (result.message || 'Unknown error'));
                }
            },
            error: () => {
                this.handleLoginError('Wrong email or password');
            }
        });
    }
    // signIn(): void {
    //     // 1) Validate
    //     if (this.signInForm.invalid) {
    //         this.notificationService.error(
    //             'Please enter a valid username and password',
    //             'Login Failed',
    //             { timeOut: 3000, positionClass: 'toast-top-right' }
    //         );
    //         return;
    //     }

    //     this.signInForm.disable();
    //     this.showAlert = false;

    //     // 2) Build credentials
    //  debugger
    //     const loginData = {
    //         username: this.signInForm.value.username,
    //         password: this.signInForm.value.password
    //     };

    //     // 3) Call AuthService.signIn() → it POSTS, writes localStorage['accessToken'], sets userService.user, _authenticated=true
    //      this.publicService.login(loginData).subscribe({
    //         next: (res) => {
    //             debugger
    //             // 4) Now stash the rest of your session info:
    //             if (res.user?.id) {
    //                 localStorage.setItem('userId', res.user.id.toString());
    //             }
    //             localStorage.setItem('userPermissions', JSON.stringify(res.permissions || []));
    //             const name = res.user?.name || res.user?.userName || '';
    //             localStorage.setItem('Name', name);

    //             // 5) Refresh your permission cache
    //             this.permissionService.setPermissions();

    //             // 6) Re-enable UI and redirect
    //             this.signInForm.enable();
    //             const redirectURL =
    //                 this._activatedRoute.snapshot.queryParamMap.get('redirectURL')
    //                 || '/signed-in-redirect';
    //             this._router.navigateByUrl(redirectURL);
    //         },
    //         error: (err) => {
    //             // 7) On error, show alert and re-enable form
    //             this.signInForm.enable();
    //             this.alert = { type: 'error', message: err?.message || 'Login failed' };
    //             this.showAlert = true;
    //             this.notificationService.error(this.alert.message, 'Login Failed', {
    //                 timeOut: 3000,
    //                 positionClass: 'toast-top-right'
    //             });
    //         }
    //     });
    // }



    private handleLoginError(message: string): void {
        this.signInForm.enable();
        // this.signInNgForm.resetForm(); // REMOVE this line to keep username/password
        this.alert = {
            type: 'error',
            message: message,
        };
        this.showAlert = true;

        // Show toast
        // Show toast using ngx-toastr
        this.notificationService.error(message, 'Login Failed', {
            timeOut: 3000,
            positionClass: 'toast-top-right'
        });
    }

}