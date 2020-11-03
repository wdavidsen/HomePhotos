import { $$, $, ElementFinder, by } from 'protractor';

export class UserDetailPage {
    componentCss = 'app-user-detail';
    titleCss = 'app-user-detail > h2';
    summaryInfoCss = 'app-user-detail .summary-info p';
    userIdBoxCss = '#usernameInput';
    passwordBoxCss = '#passwordInput';
    passwordCompareBoxCss = '#passwordCompareInput';
    firstNameBoxCss = '#firstNameInput';
    lastNameBoxCss = '#lastNameInput';
    roleSelectCss = '#roleSelect';
    enabledCheckCss = '#enabledCheck';
    changePasswordCheckCss = '#mustChangePasswordCheck';
    saveButtonCss = 'app-user-detail button[type=submit]';
    deleteButtonCss = 'app-user-detail button:nth-child(2)';
    resetPasswordButtonCss = 'app-user-detail button:nth-child(3)';
    getReturnLinkCss = 'app-user-detail a[href="/users"]';

    getTitle(): ElementFinder {
        return $(this.titleCss);
    }

    getSummaryInfo(): ElementFinder {
        return $(this.summaryInfoCss);
    }

    getUserIdBox(): ElementFinder {
        return $(this.userIdBoxCss);
    }

    getPasswordBox(): ElementFinder {
        return $(this.passwordBoxCss);
    }

    getPasswordCompareBox(): ElementFinder {
        return $(this.passwordCompareBoxCss);
    }

    getFirstNameBox(): ElementFinder {
        return $(this.firstNameBoxCss);
    }

    getLastNameBox(): ElementFinder {
        return $(this.lastNameBoxCss);
    }

    getRoleSelect(): ElementFinder {
        return $(this.roleSelectCss);
    }

    getEnabledCheck(): ElementFinder {
        return $(this.enabledCheckCss);
    }

    getMustChangePasswordCheck(): ElementFinder {
        return $(this.changePasswordCheckCss);
    }

    getSaveButton(): ElementFinder {
        return $(this.saveButtonCss);
    }

    getDeleteButton(): ElementFinder {
        return $(this.deleteButtonCss);
    }

    getResetPasswordButton(): ElementFinder {
        return $(this.resetPasswordButtonCss);
    }

    getReturnLink(): ElementFinder {
        return $(this.getReturnLinkCss);
    }

    async selectRole(name: string): Promise<void> {
        await $(this.roleSelectCss).element(by.cssContainingText('option', name)).click();
    }
}
