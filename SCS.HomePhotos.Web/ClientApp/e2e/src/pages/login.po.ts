import { $, $$, browser, by, element } from 'protractor';

export class LoginPage {
  usernameInputCss = 'input[formcontrolname=username]';
  passwordInputCss = 'input[formcontrolname=password]';
  loginButtonCss = 'button.btn-primary';
  registerLinkCss = 'a[routerlink="/register"]';
  invalidTextCss = '.invalid-feedback';
  headerTextCss = '.app-container h2';

  navigateTo() {
    return browser.get('/login');
  }

  getMainHeading() {
    return $(this.headerTextCss).getText();
  }

  login(usernmae, password) {
    const usernameInput = $(this.usernameInputCss);
    const passwordInput = $(this.passwordInputCss);

    usernameInput.sendKeys(usernmae);
    passwordInput.sendKeys(password);

    const loginButton = $(this.loginButtonCss);

    return loginButton.click();
  }

  register() {
    const registerLink = $(this.registerLinkCss);

    return registerLink.click();
  }

  async getInvalidMessages() {
    const messages: string[] = [];

    await $$(this.invalidTextCss).each(async (e) => messages.push(await e.getText()));
    return messages;
  }
}
