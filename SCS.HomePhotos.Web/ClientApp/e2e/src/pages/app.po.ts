import { $, browser, by, element } from 'protractor';

export class AppPage {
  organizeCheckCss = '#organizeSection > app-organize > input';

  navigateTo() {
    return browser.get('/');
  }

  getTitle() {
    return $('.navbar-brand').getText();
  }

  navigate(pageName: string) {
    const menuLink = $(`a[routerlink="/${pageName}"]`);

    return menuLink.click();
  }

  async toggleOrganizeMode() {
    await $(this.organizeCheckCss).click();
  }

  async setOrganizeMode(enabled: boolean) {
    const checkbox = $(this.organizeCheckCss);

    if (!await checkbox.isSelected() && enabled) {
      await checkbox.click();
    }
    else if (await checkbox.isSelected() && !enabled) {
      await checkbox.click();
    }
  }
}
