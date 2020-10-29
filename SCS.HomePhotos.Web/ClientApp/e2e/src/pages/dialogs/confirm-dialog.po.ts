import { $, ElementFinder, protractor } from 'protractor';

export class ConfirmDialog {
    EC = protractor.ExpectedConditions;

    yesButtonCss = '.modal-footer button:nth-child(1)';
    noButtonCss = '.modal-footer button:nth-child(2)';
    cancelButtonCss = '.modal-footer button:nth-child(3)';

    getYesButton(): ElementFinder {
        return $(this.yesButtonCss);
    }

    getNoButton(): ElementFinder {
        return $(this.noButtonCss);
    }

    getCancelButton(): ElementFinder {
        return $(this.cancelButtonCss);
    }
}
