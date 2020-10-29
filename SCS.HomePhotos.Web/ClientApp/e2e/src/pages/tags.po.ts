import { $, $$, browser, ElementFinder, promise, protractor } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class TagsPage {
    EC = protractor.ExpectedConditions;
    // tagsCss = '.tag-cloud > div > div.tag';
    tagsCss = 'app-tags > section > nav';
    toolbarCss = 'nav.navbar.fixed-bottom';
    addModalCss = '.tag-add-dialog';
    deleteModalCss = '.confirm-delete-dialog';
    renameModalCss = '.tag-rename-dialog';
    copyModalCss = '.tag-copy-dialog';
    combineModalCss = '.tag-combine-dialog';

    navigateTo() {
        return browser.get('/tags');
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

    clickTag(index: number): PromiseVoid {
        const tag = this.getTag(index);
        return tag.click();
    }

    async selectTag(tagName: string): Promise<void> {
        (await this.getTagNames()).forEach((name, index) => {
            if (name === tagName) {
                return this.clickTag(index);
            }
        });
    }

    async isSelected(index: number): Promise<boolean> {
        const photo = this.getTag(index);
        const classes = await photo.getAttribute('class');
        return classes.split(' ').some(c => c === 'tag-selected');
    }

    getTag(index: number): ElementFinder {
        return $(this.tagsCss + ` > div:nth-child(${index + 1}) > div.tag`);
    }

    getToolbar(): ElementFinder {
        return $(this.toolbarCss);
    }

    getAddButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(1)');
    }

    getDeleteButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(2)');
    }

    getRenameButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(3)');
    }

    getCopyButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(4)');
    }

    getCombineButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(5)');
    }

    getClearButton(): ElementFinder {
        return $(this.toolbarCss + ' button.navbar-btn:nth-child(6)');
    }
}
