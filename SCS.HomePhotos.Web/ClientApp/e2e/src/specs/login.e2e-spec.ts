import { $, browser, protractor } from 'protractor';
import { E2eUtil } from '../e2e-util';
import { AppPage } from '../pages/app.po';
import { LoginPage } from '../pages/login.po';

describe('Login', () => {
  let loginPage: LoginPage;
  let appPage: AppPage;

  beforeEach(async () => {
    loginPage = new LoginPage();
    appPage = new AppPage();
    await loginPage.navigateTo();
  });

  it('should navigate to registration page', async () => {
    await loginPage.getRegisterLink().click();
    E2eUtil.browserWaitFor(loginPage.registerHeadingCss);
  });

  it('should login with valid cred', async () => {
    await loginPage.login('wdavidsen', 'Pass@123');
    E2eUtil.browserWaitFor('.photo-list');
  });

  it('should not login with blank username', async () => {
    await loginPage.login('', 'password1');

    const messages = await loginPage.getInvalidMessages();
    expect(messages.length).toBe(1);
    expect(messages).toContain('Username is required');
  });

  it('should not login with blank password', async () => {
    await loginPage.login('wdavidsen', '');

    const messages = await loginPage.getInvalidMessages();
    expect(messages.length).toBe(1);
    expect(messages).toContain('Password is required');
  });
});
