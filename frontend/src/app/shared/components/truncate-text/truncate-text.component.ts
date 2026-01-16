import { Input, Component } from "@angular/core";

@Component({
    selector: 'truncate-text',
    templateUrl: './truncate-text.component.html'
})

export class TruncateTextComponent {
    @Input() text: string = "";
    @Input() limit: number = 40;
    truncating = true;
}