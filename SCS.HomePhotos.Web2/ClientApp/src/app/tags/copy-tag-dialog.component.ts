import { OnInit, Component } from '@angular/core';
import { BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-copy-tag-dialog',
    templateUrl: './copy-tag-dialog.component.html',
    styleUrls: ['./copy-tag-dialog.component.css']
  })

export class CopyTagDialogComponent {
    title: string;
    message: string;
    label: string;
    tagName: string;
    tagType: string = 'S';
    okClicked: boolean;
    cancelClicked: boolean; 
    showTagType: boolean;  

    constructor(private bsModalRef: BsModalRef) {}

    static GetOptions(className: string, title: string, label: string, message: string, tagName: string, tagType: string, showTagType: boolean): ModalOptions {
        return {
            class: className,
            ignoreBackdropClick: false,
            keyboard: true,
            initialState: {
              title: title,
              label: label,
              message: message,
              tagName: tagName,              
              tagType: tagType,
              okClicked: false,
              cancelClicked: false,
              showTagType: showTagType
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
