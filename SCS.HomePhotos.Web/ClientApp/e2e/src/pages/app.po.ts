import { $, browser, ElementFinder, promise } from 'protractor';

type PromiseVoid = promise.Promise<void>;
type PromiseString = promise.Promise<string>;

export class AppPage {
  organizeCheckCss = '#organizeSection > app-organize > input';
  logoutLinkCss = '#rightMenu > a:nth-child(3)';
  accountLinkCss = '#rightMenu > a:nth-child(5)';

  navigateTo(): PromiseVoid {
    return browser.get('/');
  }

  getTitle(): PromiseString {
    return $('.navbar-brand').getText();
  }

  navigate(pageName: string): PromiseVoid {
    const menuLink = $(`a[routerlink="/${pageName}"]`);

    return menuLink.click();
  }

  getLogoutLink(): ElementFinder {
    return $(this.logoutLinkCss);
  }

  async toggleOrganizeMode(): Promise<void> {
    await $(this.organizeCheckCss).click();
  }

  async setOrganizeMode(enabled: boolean): Promise<void> {
    const checkbox = $(this.organizeCheckCss);

    if (!await checkbox.isSelected() && enabled) {
      await checkbox.click();
    }
    else if (await checkbox.isSelected() && !enabled) {
      await checkbox.click();
    }
  }
}
