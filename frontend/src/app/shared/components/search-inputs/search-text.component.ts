import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { UntypedFormControl } from '@angular/forms';

import { debounceTime } from 'rxjs/operators';

@Component({
    selector: 'search-text',
    template: ``,
    styles: [`
        
        ::ng-deep  .mat-mdc-form-field-bottom-align{
            display: none !important;
         }
      `]
})
export class SearchTextComponent implements OnInit {
    searchBox: UntypedFormControl = new UntypedFormControl();

    @Input() placeHolder = "Search";
    @Output() textSearchEntered = new EventEmitter();

    ngOnInit() {
        this.searchBox.valueChanges.pipe(debounceTime(300))
            .subscribe(value => this.textSearchEntered.emit(value));
    }
}