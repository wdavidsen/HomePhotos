import { $, $$, ElementFinder, protractor } from 'protractor';

export class PhotoTaggerDialog {
    EC = protractor.ExpectedConditions;

    inputCss = 'app-photo-tagger .modal-body #tagSearchTypeahead';
    addButtonCss = 'app-photo-tagger .modal-body button:nth-child(1)';
    okButtonCss = 'app-photo-tagger .modal-footer button:nth-child(1)';
    closeButtonCss = 'app-photo-tagger .modal-footer button:nth-child(2)';
    tagChecksCss = 'app-photo-tagger app-tri-check';

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

    async getTagNames(): Promise<string[]> {
        // body > modal-container > div > div > app-photo-tagger > div.modal-body > div > app-tri-check:nth-child(1) > div > label
        const names: string[] = [];

        await $$(this.tagChecksCss + ' label').each(async (e) => {
            let text = await e.getText();
            text = text.trim();
            names.push(text);
        });
        return names;
    }

    async getTagState(tagName: string): Promise<number> {
        const tagNames = await this.getTagNames();
        const tagIndex = tagNames.indexOf(tagName);
        const tagState = $(this.tagChecksCss + `:nth-child(${tagIndex})`);

        if (tagState.$('.tricheck-on').isPresent()) {
            return 1;
        }
        if (tagState.$('.tricheck-indeterminate').isPresent()) {
            return 3;
        }
        return 0;
    }
}
