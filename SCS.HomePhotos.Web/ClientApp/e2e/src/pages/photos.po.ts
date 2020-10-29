import { $, $$, browser, ElementArrayFinder, ElementFinder, promise, protractor } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class PhotosPage {
    EC = protractor.ExpectedConditions;
    photosCss = '.photo-list div';
    shadowboxCss = '.blueimp-gallery .slides';
    toolbarCss = 'nav.navbar.fixed-bottom';
    taggerModalCss = 'app-photo-tagger';

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
        return classes.split(' ').some(c => c === 'selected');
    }

    getPhotos(): ElementArrayFinder {
        return $$(this.photosCss);
    }

    getSelectedPhotos(): ElementArrayFinder {
        return $$(this.photosCss + '.selected');
    }

    getPhoto(index: number): ElementFinder {
        return $(this.photosCss + `:nth-child(${index + 1})`);
    }

    getToolbar(): ElementFinder {
        return $(this.toolbarCss);
    }

    getTagPhotosButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(1)');
    }

    getSelectAllButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(2)');
    }

    getClearButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(3)');
    }

    getTaggerModal(): ElementFinder {
        return $(this.taggerModalCss);
    }
}
