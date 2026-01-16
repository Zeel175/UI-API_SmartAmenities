import { Directive, Input } from '@angular/core'
@Directive({
    selector: 'img',
    host: {
        '(error)': 'updateUrl()',
        '[src]': 'src'
    }
})

export class ImageFallbackDirective {
    @Input() src: string = "";
    @Input() default: string = 'assets/images/no-image.png';

    updateUrl() {
        this.src = this.default;
    }
}