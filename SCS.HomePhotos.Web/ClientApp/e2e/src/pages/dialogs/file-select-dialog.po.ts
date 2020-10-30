import { $, ElementFinder, protractor } from 'protractor';

export class FileSelectDialog {
    EC = protractor.ExpectedConditions;

    fileInputCss = '.file-select-dialog .modal-body input[type=file]';
    closeButtonCss = '.file-select-dialog .modal-header button:nth-child(1)';

    getFileInputBox(): ElementFinder {
        return $(this.fileInputCss);
    }

    getCloseButton(): ElementFinder {
        return $(this.closeButtonCss);
    }
}
