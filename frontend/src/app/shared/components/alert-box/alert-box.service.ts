

import { Injectable } from "@angular/core";
import { BsModalService, BsModalRef } from "ngx-bootstrap/modal";
import { AlertBoxComponent } from "./alert-box.component";

@Injectable()
export class AlertBoxService {
    alertModalRef: BsModalRef;

    constructor(private modalService: BsModalService) {

    }

    show(data: { message: string }) {
        const initialState = {
            message: data.message
        }

        this.alertModalRef = this.modalService.show(AlertBoxComponent, {
            backdrop: true,
            ignoreBackdropClick: true,
            keyboard:false,
            initialState
        });

        this.alertModalRef.content.onClose.subscribe((result: any) => {
            this.alertModalRef.content.onClose.unsubscribe();
        });
    }
}