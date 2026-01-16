import { Component, OnInit } from '@angular/core';
// import { UserAuthService, AccountService } from '@app-core';
import { UserAuthService, AccountService } from 'app/core';
import { Router } from '@angular/router';
import { AuthService } from 'app/core/auth/auth.service';

@Component({
    selector: 'logout',
    templateUrl: './logout.component.html'
})

export class LogoutComponent implements OnInit {

    constructor(private router: Router, private accountService: AccountService,
       // private userAuthService: UserAuthService
        private authService: AuthService) {
    }

    ngOnInit() {
        this.logout();
    }

    logout() {
       
                this.authService.loggedOut();
                 localStorage.removeItem('userId');
                setTimeout(() => {
                    this.router.navigate(['login']);
                });
            }
}