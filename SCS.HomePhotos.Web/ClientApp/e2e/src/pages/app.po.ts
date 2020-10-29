import { $, browser, by, element, promise } from 'protractor';

type PromiseVoid = promise.Promise<void>;
type PromiseString = promise.Promise<string>;

export class AppPage {
  organizeCheckCss = '#organizeSection > app-organize > input';

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
