import { NgForm } from "@angular/forms";
import { CanDeactivateComponent } from "../can-deactivate/can-deactivate.component";
import { Directive } from "@angular/core";

@Directive()
export abstract class FormCanDeactivate extends CanDeactivateComponent {

    abstract get form(): NgForm;
    abstract get canSkip(): boolean;
    abstract get forceStop(): boolean;

    canDeactivate(): boolean {
        return (!this.forceStop && (this.form.submitted || !this.form.dirty || this.canSkip));
    }
}