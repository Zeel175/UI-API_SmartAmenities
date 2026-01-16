import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { CanDeactivateComponent } from './can-deactivate.component';
import { Observable } from 'rxjs';

@Injectable()
export class CanDeactivateGuard implements CanDeactivate<CanDeactivateComponent> {
    canDeactivate(component: CanDeactivateComponent): boolean | Observable<boolean> {
        if (!component.canDeactivate()) {
            if (confirm("You are about to navigate away from this transaction. Changes you made may not be saved. Do you want to proceed?")) {
                return true;
            } else {
                return false;
            }
        }

        return true;
    }
}