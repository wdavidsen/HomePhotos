import { $, browser, protractor } from 'protractor';
import { LoginPage } from '../pages/login.po';

xdescribe('Login', () => {
  let page: LoginPage;
  const EC = protractor.ExpectedConditions;

  beforeEach(() => {
    page = new LoginPage();
    page.navigateTo();
  });

  it('should display heading', () => {
    expect(page.getMainHeading()).toEqual('Sign-In');
  });

  it('should login with valid cred', async () => {
    await page.login('wdavidsen', 'Pass@123');
    browser.wait(EC.presenceOf($('.photo-list')), 5000);
  });

  it('should not login with blank username', async () => {
    await page.login('', 'password1');

    const messages = await page.getInvalidMessages();
    expect(messages.length).toBe(1);
    expect(messages).toContain('Username is required');
  });

  it('should not login with blank password', async () => {
    await page.login('wdavidsen', '');

    const messages = await page.getInvalidMessages();
    expect(messages.length).toBe(1);
    expect(messages).toContain('Password is required');
  });

  it('should navigate to registration page', async () => {
    await page.register();
    browser.wait(EC.urlContains('/register'), 5000);
  });
});
