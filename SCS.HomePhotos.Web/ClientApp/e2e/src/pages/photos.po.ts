import { $, $$, browser, ElementFinder, promise, protractor } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class PhotosPage {
    EC = protractor.ExpectedConditions;
    photosCss = '.photo-list div';

    navigateTo() {
        browser.get('/photos');
    }

    async getPhotoLinks(): Promise<string[]> {
        const links: string[] = [];

        await $$(this.photosCss).each(async (e) => {
            const style = await e.getAttribute('style');
            const url = style.substring(style.indexOf('(') + 1, style.indexOf(')') - 1);
            links.push(url);
        });
        return links;
    }

    clickPhoto(index: number): PromiseVoid {
        const photo = this.getPhoto(index);
        return photo.click();
    }

    async isSelected(index: number): Promise<boolean> {
        const photo = this.getPhoto(index);
        const classes = await photo.getAttribute('class');
        return classes.split(' ').some(c => c === '.selected');
    }

    private getPhoto(index: number): ElementFinder {
        return $(this.photosCss + `:nth-child(${index})`);
    }
}
