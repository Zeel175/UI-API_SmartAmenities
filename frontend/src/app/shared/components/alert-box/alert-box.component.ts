import { Component } from "@angular/core";
import { Subject } from "rxjs";
import { BsModalRef } from "ngx-bootstrap/modal";

@Component({
    templateUrl: "./alert-box.component.html"
})
export class AlertBoxComponent {
    onClose: Subject<boolean> ;
    message: string;
    modalTitle:string;

    constructor(public bsModalRef: BsModalRef) {
        this.onClose = new Subject();
    }

    btnOk() {
        this.onClose.next(true);
        this.bsModalRef.hide();
    }
}