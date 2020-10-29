import { $, ElementFinder, protractor } from 'protractor';

export class InputDialog {
    EC = protractor.ExpectedConditions;

    inputCss = '.modal-body input[type=text]';
    okButtonCss = '.modal-footer button:nth-child(1)';
    cancelButtonCss = '.modal-footer button:nth-child(2)';

    getInputBox(): ElementFinder {
        return $(this.inputCss);
    }

    getOkButton(): ElementFinder {
        return $(this.okButtonCss);
    }

    getCancelButton(): ElementFinder {
        return $(this.cancelButtonCss);
    }
}
