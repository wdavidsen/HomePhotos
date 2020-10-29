import { $, ElementFinder, protractor } from 'protractor';

export class AlertDialog {
    EC = protractor.ExpectedConditions;

    okButtonCss = '.modal-footer button:nth-child(1)';
    cancelButtonCss = '.modal-footer button:nth-child(2)';

    getOkButton(): ElementFinder {
        return $(this.okButtonCss);
    }

    getCancelButton(): ElementFinder {
        return $(this.cancelButtonCss);
    }
}
