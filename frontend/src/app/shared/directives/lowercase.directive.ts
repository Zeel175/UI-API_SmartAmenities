import { Directive, ElementRef, HostListener } from '@angular/core';

@Directive({
    selector: '[textlower]'
})
export class LowercaseDirective {

    constructor(public ref: ElementRef) { }

    @HostListener('input', ['$event']) onInput(event : any) {
        this.ref.nativeElement.value = event.target.value.toLowerCase();
    }
}