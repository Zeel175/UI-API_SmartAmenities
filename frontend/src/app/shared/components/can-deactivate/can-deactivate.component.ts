import { HostListener, Directive } from "@angular/core";
import { Observable } from "rxjs";

@Directive()
export abstract class CanDeactivateComponent {

    abstract canDeactivate(): boolean | Observable<boolean>;

    @HostListener('window:beforeunload', ['$event'])
    unloadNotification($event: any) {
        if (!this.canDeactivate()) {
            $event.returnValue = true;
        }
    }
}