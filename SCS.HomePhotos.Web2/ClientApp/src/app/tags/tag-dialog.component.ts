import { Component } from '@angular/core';
import { BsModalRef, ModalOptions } from 'ngx-bootstrap/modal';

@Component({
    selector: 'app-tag-dialog',
    templateUrl: './tag-dialog.component.html',
    styleUrls: ['./tag-dialog.component.css']
  })

export class TagDialogComponent {
    title: string;
    message: string;
    label: string;
    tagName: string;
    tagType: string = 'S';
    okClicked: boolean;
    cancelClicked: boolean; 
    showTagType: boolean;  
    isUpdate: boolean;  

    constructor(private bsModalRef: BsModalRef) {}

    static GetOptions(className: string, title: string, label: string, message: string, tagName: string, tagType: string, showTagType: boolean, isUpdate: boolean): ModalOptions {
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
              showTagType: showTagType,
              isUpdate: isUpdate
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
