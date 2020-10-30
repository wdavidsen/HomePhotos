import { $, $$, ElementFinder, protractor } from 'protractor';

export class UploadTaggerDialog {
    EC = protractor.ExpectedConditions;

    inputCss = 'app-upload-photo-tagger .modal-body #tagSearchTypeahead';
    addButtonCss = 'app-upload-photo-tagger .modal-body button:nth-child(1)';
    okButtonCss = 'app-upload-photo-tagger .modal-footer button:nth-child(1)';
    closeButtonCss = 'app-upload-photo-tagger .modal-header button:nth-child(1)';
    tagsCss = 'app-upload-photo-tagger .modal-body > nav';

    getInputBox(): ElementFinder {
        return $(this.inputCss);
    }

    getAddButton(): ElementFinder {
        return $$(this.addButtonCss).first();
    }

    getOkButton(): ElementFinder {
        return $(this.okButtonCss);
    }

    getCloseButton(): ElementFinder {
        return $(this.closeButtonCss);
    }

    async getTagNames(includeCount = false): Promise<string[]> {
        const names: string[] = [];

        await $$(this.tagsCss + ' > div > div.tag').each(async (e) => {
            let text = await e.getText();
            text = text.trim();

            if (!includeCount) {
                text = text.substring(0, text.lastIndexOf(' '));
            }
            names.push(text);
        });
        return names;
    }
}
