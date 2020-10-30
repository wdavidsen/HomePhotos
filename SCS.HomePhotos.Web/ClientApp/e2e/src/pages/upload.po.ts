import { $, browser, ElementFinder, promise } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class UploadPage {
    componentCss = 'app-upload';
    selectButtonCss = 'app-upload .toolbar button:nth-child(1)';
    uploadButtonCss = 'app-upload .toolbar button:nth-child(2)';
    cancelButtonCss = 'app-upload .toolbar button:nth-child(3)';
    clearButtonCss = 'app-upload .toolbar button:nth-child(4)';
    tagButtonCss = 'app-upload .toolbar button:nth-child(5)';
    fileSelectDialogCss = '.file-select-dialog';
    fileTilesCss = 'app-upload div.files div.image-tile';

    navigateTo() {
        return browser.get('/upload');
    }

    getSelectButton(): ElementFinder {
        return $(this.selectButtonCss);
    }

    getUploadButton(): ElementFinder {
        return $(this.uploadButtonCss);
    }

    getCancelButton(): ElementFinder {
        return $(this.cancelButtonCss);
    }

    getClearButton(): ElementFinder {
        return $(this.clearButtonCss);
    }

    getTagPhotosButton(): ElementFinder {
        return $(this.tagButtonCss);
    }

    getImageSuccessCss(index: number): string {
        return this.fileTilesCss + `:nth-child(${index + 2}) span.text-success`;
    }

    async getImageFileSize(index: number): Promise<string> {
        return await $(this.fileTilesCss + `:nth-child(${index + 2}) div.size`).getText();
    }

    async getImageStatus(index: number): Promise<string> {
        return await ($(this.fileTilesCss + `:nth-child(${index + 2}) span.text-success`).getText());
    }

    getImageRemoveButton(index: number): ElementFinder {
        return $(this.fileTilesCss + `:nth-child(${index + 2}) button.btn-sm`);
    }
}
