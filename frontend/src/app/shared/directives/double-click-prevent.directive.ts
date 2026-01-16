import { Directive, HostListener } from '@angular/core';

@Directive({
    selector: 'button[type=submit],.no-double-click'
})
export class ButtonNoDblClickDirective {

    constructor() { }

    @HostListener('click', ['$event'])
    clickEvent(event : any ) {
        setTimeout(() => {
            event.srcElement.setAttribute('disabled', true);
        });
        setTimeout(() => {
            event.srcElement.removeAttribute('disabled');
        }, 3000);
    }
}