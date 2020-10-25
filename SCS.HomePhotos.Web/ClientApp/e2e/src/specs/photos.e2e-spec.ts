import { $, browser, promise, protractor } from 'protractor';
import { E2eUtil } from '../e2e-util';
import { AppPage } from '../pages/app.po';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';

describe('Photos', () => {
    const appPage = new AppPage();
    const photosPage = new PhotosPage();
    const EC = protractor.ExpectedConditions;

    const shadowboxCss = '.blueimp-gallery .slides';

    beforeEach(async () => {
        const loginPage = new LoginPage();
        loginPage.navigateTo();
        await E2eUtil.browserWaitFor(loginPage.loginButtonCss);
        await loginPage.login('wdavidsen', 'Pass@123');
        await E2eUtil.browserWaitFor(photosPage.photosCss);
        await appPage.setOrganizeMode(false);
    });

    it('should show photos', async () => {
        const links = await photosPage.getPhotoLinks();

        expect(links.length).toBeGreaterThan(0);
    });

    xit('should show shadow box', async () => {
        await photosPage.clickPhoto(0);

        browser.wait(EC.presenceOf($(shadowboxCss)), 5000);
    });

    xit('should select photo', async () => {
        appPage.setOrganizeMode(true);
        await photosPage.clickPhoto(0);

        expect(photosPage.isSelected(0)).toBe(true);
    });
});
