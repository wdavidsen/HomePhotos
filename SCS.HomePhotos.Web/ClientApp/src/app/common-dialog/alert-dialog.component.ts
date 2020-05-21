import { Component } from '@angular/core';
import { BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-alert-dialog',
    templateUrl: './alert-dialog.component.html',
    styleUrls: ['./alert-dialog.component.css']
  })
export class AlertDialogComponent {
    title: string;
    message: string;

    constructor(public bsModalRef: BsModalRef) {}

    static GetOptions(title: string, message: string): ModalOptions {
        return {
            ignoreBackdropClick: false,
            keyboard: true,
            initialState: {
              title: title,
              message: message
            }
        };
    }

    ok() {
        this.bsModalRef.hide();
    }
}
