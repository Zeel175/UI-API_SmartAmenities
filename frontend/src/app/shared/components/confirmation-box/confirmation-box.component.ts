import { Component, OnInit, ViewChild, ElementRef } from "@angular/core";
import { Subject } from "rxjs";
import { BsModalRef } from "ngx-bootstrap/modal";
import { CommonModule } from "@angular/common";

@Component({
    templateUrl: "./confirmation-box.component.html",
    styleUrls: ["./confirmation-box.component.scss"],
    imports: [CommonModule],
})
export class ConfirmationBoxComponent {
    onClose: Subject<boolean> ;
    message: string;
    yesBtnText: string = 'Yes';
    noBtnText: string = 'No';
    isWarning: boolean = false;
    @ViewChild('btnYes', { static: false, read: ElementRef }) btnYes: ElementRef;
    @ViewChild('btnNo', { static: false, read: ElementRef }) btnNo: ElementRef;

    constructor(public bsModalRef: BsModalRef) {
        this.onClose = new Subject();
    }

    btnClicked(action: boolean) {
        this.btnYes.nativeElement.setAttribute('disabled', true);
        this.btnNo.nativeElement.setAttribute('disabled', true);

        this.onClose.next(action);
        this.bsModalRef.hide();
    }
}