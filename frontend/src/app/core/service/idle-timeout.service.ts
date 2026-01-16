import { Injectable, NgZone } from '@angular/core';
import { Router } from '@angular/router';

@Injectable({
    providedIn: 'root',
})
export class IdleTimeoutService {
    private timeoutId: any;
    private readonly timeoutDuration = 15 * 60 * 1000; // 15 minutes

    constructor(private router: Router, private ngZone: NgZone) {
        this.startWatching();
    }

    private startWatching() {
        this.resetTimeout();
        ['click', 'mousemove', 'keydown', 'scroll', 'touchstart'].forEach(event => {
            window.addEventListener(event, () => this.resetTimeout());
        });
    }

    private resetTimeout() {
        clearTimeout(this.timeoutId);

        
        if (this.router.url === '/login') {
            return; 
        }

        this.timeoutId = setTimeout(() => this.handleTimeout(), this.timeoutDuration);
    }

    private handleTimeout() {
        this.ngZone.run(() => {
            alert('Session expired due to inactivity. Redirecting to login page.');
            this.router.navigate(['/login']);
        });
    }
}