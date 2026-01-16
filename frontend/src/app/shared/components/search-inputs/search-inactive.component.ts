import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';

@Component({
    selector: 'search-inactive',
    template: ` <div class="m-checkbox-inline">
            <!-- <label class="m-checkbox">
                <input type="checkbox" [formControl]="searchBox" (change)="checkboxChanged($event.currentTarget)"> {{placeHolder}}
                <span></span>
            </label> -->
        </div>`,
    styles: []
})

export class SearchInActiveComponent {

    searchBox: UntypedFormControl = new UntypedFormControl(false);

    @Input() placeHolder = "Show inactive";
    @Output() chkChanged = new EventEmitter();

    checkboxChanged(target: EventTarget) {
        const isChecked: boolean = (target as HTMLInputElement).checked;
        this.chkChanged.emit(isChecked ? true : false);
    }
}