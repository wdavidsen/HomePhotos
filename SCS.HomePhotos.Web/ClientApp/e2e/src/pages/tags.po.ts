import { $, $$, browser, ElementFinder, promise, protractor } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class TagsPage {
    EC = protractor.ExpectedConditions;
    tagsCss = '.tag-cloud .tag';

    navigateTo() {
        return browser.get('/tags');
    }

    async getTagNames(): Promise<string[]> {
        const names: string[] = [];

        await $$(this.tagsCss).each(async (e) => {
            const text = await e.getText();
            names.push(text);
        });
        return names;
    }

    clickTag(index: number): PromiseVoid {
        const tag = this.getTag(index);
        return tag.click();
    }

    async isSelected(index: number): Promise<boolean> {
        const photo = this.getTag(index);
        const classes = await photo.getAttribute('class');
        return classes.split(' ').some(c => c === '.tag-selected');
    }

    private getTag(index: number): ElementFinder {
        return $(this.tagsCss + `:nth-child(${index})`);
    }
}
