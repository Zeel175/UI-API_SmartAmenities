import { Directive, Input } from "@angular/core";

@Directive({
    selector: '[limit-to]',
    host: {
        '(keypress)': '_onKeyPress($event)'
    }
})
export class LimitToDirective {
    @Input('limit-to') limitTo : any;

    _onKeyPress(e : any) {
        const limit = +this.limitTo;
        if (e.target.value.length === limit) {
            e.preventDefault();
        }
    }
}