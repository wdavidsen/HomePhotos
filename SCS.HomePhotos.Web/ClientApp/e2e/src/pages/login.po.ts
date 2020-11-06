import { $, $$, browser, ElementFinder, promise, protractor } from 'protractor';

type PromiseVoid = promise.Promise<void>;
type PromiseString = promise.Promise<string>;

export class LoginPage {
  EC = protractor.ExpectedConditions;

  usernameInputCss = 'input[formcontrolname=username]';
  passwordInputCss = 'input[formcontrolname=password]';
  loginButtonCss = 'button.btn-primary';
  registerLinkCss = 'a.btn-link';
  invalidTextCss = '.invalid-feedback';
  headerTextCss = '.app-container h2';
  registerHeadingCss = 'form.register-form';

  navigateTo(): PromiseVoid {
    return browser.get('/login');
  }

  getMainHeading(): PromiseString {
    return $(this.headerTextCss).getText();
  }

  async login(usernmae, password): Promise<void> {
    const usernameInput = $(this.usernameInputCss);
    const passwordInput = $(this.passwordInputCss);

    usernameInput.sendKeys(usernmae);
    passwordInput.sendKeys(password);

    const loginButton = $(this.loginButtonCss);

    await loginButton.click();
  }

  getRegisterLink(): ElementFinder {
    return $(this.registerLinkCss);
  }

  async getInvalidMessages(): Promise<string[]> {
    const messages: string[] = [];

    await $$(this.invalidTextCss).each(async (e) => messages.push(await e.getText()));
    return messages;
  }
}
