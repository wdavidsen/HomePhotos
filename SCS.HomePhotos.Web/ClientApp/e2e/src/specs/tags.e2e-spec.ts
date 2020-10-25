import { $, browser, protractor } from 'protractor';
import { E2eUtil } from '../e2e-util';
import { AppPage } from '../pages/app.po';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';
import { TagsPage } from '../pages/tags.po';

describe('Tags', () => {
    const appPage = new AppPage();
    const photosPage = new PhotosPage();
    const tagsPage = new TagsPage();
    const EC = protractor.ExpectedConditions;

    beforeEach(async () => {
        const loginPage = new LoginPage();
        loginPage.navigateTo();
        await E2eUtil.browserWaitFor(loginPage.loginButtonCss);
        await loginPage.login('wdavidsen', 'Pass@123');
        await E2eUtil.browserWaitFor(photosPage.photosCss);
        await tagsPage.navigateTo();
        await E2eUtil.browserWaitFor('.tag-cloud');
        await appPage.setOrganizeMode(false);
    });

    it('should show tags', async () => {
        const tags = await tagsPage.getTagNames();

        expect(tags.length).toBeGreaterThan(0);
    });

    xit('should show photos by tag', async () => {
        await tagsPage.clickTag(0);

        browser.wait(EC.urlContains('/photos'), 5000);
    });

    xit('should select tag', async () => {
        appPage.setOrganizeMode(true);
        await tagsPage.clickTag(0);

        expect(tagsPage.isSelected(0)).toBe(true);
    });
});
