import { Directive, ElementRef, Input, OnChanges, SimpleChanges, Renderer2 } from '@angular/core';

@Directive({ selector: '[imgPreview]' })
export class ImagePreviewDirective implements OnChanges {

    @Input()
    media: any;

    constructor(private el: ElementRef, private renderer: Renderer2) { }

    ngOnChanges(changes: SimpleChanges) {

        let reader = new FileReader();
        let el = this.el;
        reader.onloadend = function (e) {
            el.nativeElement.src = reader.result;
        };

        if (this.media) {
            if (el.nativeElement.tagName === "IMG" && this.media.type.indexOf("image/") === -1) {
                el.nativeElement.src = "assets/images/pdf.png";
            } else if (this.media.type.indexOf("image/") > -1) {
                return reader.readAsDataURL(this.media);
            } else {
                // return reader.readAsDataURL(this.media);
                el.nativeElement.src = window.URL.createObjectURL(new Blob([this.media], { "type": "application/pdf" }));
            }
        }
    }
}
