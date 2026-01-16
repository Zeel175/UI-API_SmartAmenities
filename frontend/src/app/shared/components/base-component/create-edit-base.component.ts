import { ActivatedRoute, Router } from "@angular/router";
// import { BadRequestError } from "@app-models";
import { BadRequestError } from "app/model";

export abstract class CreateEditBaseComponent {

    isEditMode: boolean;
    isFormSubmitted: boolean;
    error: BadRequestError;

    constructor(public activatedRoute: ActivatedRoute, public router: Router) {
    }

    cancel() {
        if (this.isEditMode) {
            this.router.navigate(['../..', 'list'], { relativeTo: this.activatedRoute });
        }
        else {
            this.router.navigate(['..', 'list'], { relativeTo: this.activatedRoute });
        }
    }
}
