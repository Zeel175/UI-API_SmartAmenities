import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl } from '@angular/forms';
import { ValidationService } from './validation-service';
import { MatFormFieldModule, matFormFieldAnimations } from '@angular/material/form-field';


@Component({
    selector: 'mat-validation-message',
    standalone: true,
    imports: [CommonModule, MatFormFieldModule],
    template: `
     <div class="mat-form-field-subscript-wrapper outside-error" *ngIf="errorMessage !== null">
        <div [@transitionMessages]="errorMessage !== null ? 'enter' : ''">
            <mat-error class="mat-error" role="alert">{{errorMessage}}</mat-error>
        </div>
    </div>
    `,
    styles: [`
        .mat-error {
            font-size: 11px;
        }

         /*
          Position the wrapper absolutely so showing the error does not alter the
          field's height. top: 100% places it directly below the form field.
        */
        .outside-error {
            position: absolute;
            top: 100%;
            left: 0;
            width: 100%;
            margin-top: 2px;
        }
    `],
    animations: [matFormFieldAnimations.transitionMessages] // Updated usage
})
export class MatValidationMessage {

    @Input("control")
    control: AbstractControl;

    @Input("message")
    message: string = "";

    @Input("formSubmitted")
    formSubmitted: boolean = false;

    constructor() { }

    get errorMessage() {
        for (let propertyName in this.control.errors) {
            if (this.control.errors.hasOwnProperty(propertyName) && (this.control.touched || this.formSubmitted)) {
                return ValidationService.getValidatorErrorMessage(propertyName, this.control.errors[propertyName], this.message);
            }
        }
        return null;
    }
}
