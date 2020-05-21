import { OnInit, Component } from '@angular/core';
import { BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-input-dialog',
    templateUrl: './input-dialog.component.html',
    styleUrls: ['./input-dialog.component.css']
  })

export class InputDialogComponent {
    title: string;
    message: string;
    label: string;
    input: string;
    okClicked: boolean;
    cancelClicked: boolean;

    constructor(private bsModalRef: BsModalRef) {}

    static GetOptions(title: string, label: string, message: string, defaultValue: string): ModalOptions {
        return {
            ignoreBackdropClick: false,
            keyboard: true,
            initialState: {
              title: title,
              label: label,
              message: message,
              input: defaultValue,
              okClicked: false,
              cancelClicked: false
            }
        };
    }

    ok() {
        this.okClicked = true;
        this.cancelClicked = !this.okClicked;
        this.bsModalRef.hide();
    }

    cancel() {
        this.cancelClicked = true;
        this.okClicked = !this.cancelClicked;
        this.bsModalRef.hide();
    }
}
