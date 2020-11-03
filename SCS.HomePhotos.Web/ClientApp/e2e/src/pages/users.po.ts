import { $$, $, ElementFinder, promise, browser, by } from 'protractor';

type PromiseVoid = promise.Promise<void>;

export class UsersPage {
    componentCss = 'app-users';
    userTableCss = 'app-users table';
    userTableCssFormat = 'app-users table tbody tr:nth-child({num})';
    toolbarCss = 'app-users nav.navbar.fixed-bottom';
    newButtonCss = 'app-users .navbar-nav button:nth-child(1)';
    editButtonCss = 'app-users .navbar-nav button:nth-child(2)';
    deleteButtonCss = 'app-users .navbar-nav button:nth-child(3)';
    enableButtonCss = 'app-users .navbar-nav button:nth-child(4)';
    disableButtonCss = 'app-users .navbar-nav button:nth-child(5)';

    navigateTo() {
        return browser.get('/users');
    }

    getRow(index: number): ElementFinder {
        return $$(this.userTableCssFormat.replace('{num}', (index + 1).toString())).first();
    }

    async getUserRowIndex(userName: string): Promise<number> {
        let userIndex = -1;
        await $$(this.userTableCss + ' tbody tr')
            .each(async (elemFinder, idx) => {
                const text = await elemFinder.$('td:nth-child(1)').getText();
                if (text === userName) {
                    userIndex = idx;
                }
            });
        return userIndex;
    }

    async getRowData(index: number): Promise<any> {
        const row = this.getRow(index);
        const data = {
            userId: await row.$('td:nth-child(1)').getText(),
            firstName: await row.$('td:nth-child(2)').getText(),
            lastName: await row.$('td:nth-child(3)').getText(),
            lastLogin: await row.$('td:nth-child(4)').getText(),
            failedLogins: await row.$('td:nth-child(5)').getText(),
            enabled: await row.$('td:nth-child(6)').getText(),
            role: await row.$('td:nth-child(7)').getText()
        };
        return data;
    }

    clickRow(index: number): PromiseVoid {
        const row = this.getRow(index);
        return row.click();
    }

    getToolbar(): ElementFinder {
        return $(this.toolbarCss);
    }

    getNewButton(): ElementFinder {
        return $(this.newButtonCss);
    }

    getEditButton(): ElementFinder {
        return $(this.editButtonCss);
    }

    getDeleteButton(): ElementFinder {
        return $(this.deleteButtonCss);
    }

    getEnableButton(): ElementFinder {
        return $(this.enableButtonCss);
    }

    getDisableButton(): ElementFinder {
        return $(this.disableButtonCss);
    }
}
