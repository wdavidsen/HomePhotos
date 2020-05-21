import { Component } from '@angular/core';
import { BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './confirm-dialog.component.html',
    styleUrls: ['./confirm-dialog.component.css']
  })

export class ConfirmDialogComponent {
    title: string;
    message: string;
    showCancel: boolean;
    yesClicked: boolean;
    noClicked: boolean;
    cancelClicked: boolean;

    constructor(private bsModalRef: BsModalRef) {}

    static GetOptions(title: string, message: string, showCancel: boolean): ModalOptions {
        return {
            ignoreBackdropClick: !showCancel,
            keyboard: showCancel,
            initialState: {
              title: title,
              message: message,
              showCancel: showCancel,
              yesClicked: false,
              noClicked: false,
              cancelClicked: false
            }
        };
    }

    yes() {
        this.yesClicked = true;
        this.noClicked = this.cancelClicked = !this.yesClicked;
        this.bsModalRef.hide();
    }

    no() {
        this.noClicked = true;
        this.yesClicked = this.cancelClicked = !this.noClicked;
        this.bsModalRef.hide();
    }

    cancel() {
        this.cancelClicked = true;
        this.yesClicked = this.noClicked = !this.cancelClicked;
        this.bsModalRef.hide();
    }
}
