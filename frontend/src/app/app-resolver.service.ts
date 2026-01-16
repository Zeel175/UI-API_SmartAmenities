import { Injectable } from '@angular/core';

import { Observable, of } from 'rxjs';
@Injectable({
    providedIn: 'root'  // Add this back
})
export class AppResolver  {
    constructor() { }

    resolve(): Observable<any> {
            return of(null);
    }
}