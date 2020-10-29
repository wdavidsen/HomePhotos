import { E2eUtil } from '../e2e-util';
import { AppPage } from '../pages/app.po';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';

describe('Photos', () => {
    const appPage = new AppPage();
    const photosPage = new PhotosPage();

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

    it('should show shadow box', async () => {
        await photosPage.clickPhoto(0);

        E2eUtil.browserWaitFor(photosPage.shadowboxCss);
    });

    it('should select photo', async () => {
        await appPage.setOrganizeMode(true);
        await photosPage.clickPhoto(0);

        expect(photosPage.isSelected(0)).toBe(true);
    });

    it('should toggle toolbar', async () => {
        await appPage.setOrganizeMode(true);
        const toolbar = photosPage.getToolbar();

        expect(toolbar.isDisplayed()).toBe(true);
        await appPage.setOrganizeMode(false);
        expect(toolbar.isDisplayed()).toBe(false);
    });

    it('should toggle photo selection', async () => {
        await appPage.setOrganizeMode(true);
        const selectButton = photosPage.getSelectAllButton();
        const clearButton = photosPage.getClearButton();

        await selectButton.click();
        let photos = photosPage.getSelectedPhotos();
        expect(photos.count()).toBeGreaterThan(0);

        await clearButton.click();
        photos = photosPage.getSelectedPhotos();
        expect(photos.count()).toBe(0);
    });

    it('should show photo tagger', async () => {
        await appPage.setOrganizeMode(true);
        await photosPage.clickPhoto(0);

        const tagButton = photosPage.getTagPhotosButton();
        expect(tagButton.isEnabled()).toBe(true);
        await tagButton.click();
        expect(photosPage.getTaggerModal().isDisplayed()).toBe(true);
    });
});
