import { E2eUtil } from '../e2e-util';
import { AppPage } from '../pages/app.po';
import { ConfirmDialog } from '../pages/dialogs/confirm-dialog.po';
import { InputDialog } from '../pages/dialogs/input-dialog.po';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';
import { TagsPage } from '../pages/tags.po';

describe('Tags', () => {
    const appPage = new AppPage();
    const photosPage = new PhotosPage();
    const tagsPage = new TagsPage();
    const inputDialog = new InputDialog();
    const confirmDialog = new ConfirmDialog();

    beforeEach(async () => {
        const loginPage = new LoginPage();
        loginPage.navigateTo();
        await E2eUtil.browserWaitFor(loginPage.loginButtonCss);
        await loginPage.login('wdavidsen', 'Pass@123');
        await E2eUtil.browserWaitFor(photosPage.photosCss);
        await tagsPage.navigateTo();
        await E2eUtil.browserWaitFor(tagsPage.tagsCss);
        await appPage.setOrganizeMode(false);
    });

    it('should show tags', async () => {
        const tags = await tagsPage.getTagNames();

        expect(tags.length).toBeGreaterThan(0);
    });

    it('should show photos by tag', async () => {
        await tagsPage.clickTag(0);

        await E2eUtil.browserWaitFor(photosPage.photosCss);
    });

    it('should select tag', async () => {
        appPage.setOrganizeMode(true);
        await tagsPage.clickTag(0);

        expect(tagsPage.isSelected(0)).toBe(true);
    });

    it('should toggle toolbar', async () => {
        await appPage.setOrganizeMode(true);
        const toolbar = tagsPage.getToolbar();

        expect(toolbar.isDisplayed()).toBe(true);
        await appPage.setOrganizeMode(false);
        expect(toolbar.isDisplayed()).toBe(false);
    });

    it('should disable buttons with no selections', async () => {
        appPage.setOrganizeMode(true);

        expect(tagsPage.getDeleteButton().isEnabled()).toBe(false);
        expect(tagsPage.getRenameButton().isEnabled()).toBe(false);
        expect(tagsPage.getCopyButton().isEnabled()).toBe(false);
        expect(tagsPage.getCombineButton().isEnabled()).toBe(false);
        expect(tagsPage.getClearButton().isEnabled()).toBe(false);
    });

    it('should enable buttons with one selections', async () => {
        appPage.setOrganizeMode(true);
        await tagsPage.clickTag(0);

        expect(tagsPage.getDeleteButton().isEnabled()).toBe(true);
        expect(tagsPage.getRenameButton().isEnabled()).toBe(true);
        expect(tagsPage.getCopyButton().isEnabled()).toBe(true);
        expect(tagsPage.getCombineButton().isEnabled()).toBe(false);
        expect(tagsPage.getClearButton().isEnabled()).toBe(true);
    });

    it('should enable/disable buttons with multi-selections', async () => {
        appPage.setOrganizeMode(true);
        await tagsPage.clickTag(0);
        await tagsPage.clickTag(1);

        expect(tagsPage.getDeleteButton().isEnabled()).toBe(true);
        expect(tagsPage.getRenameButton().isEnabled()).toBe(false);
        expect(tagsPage.getCopyButton().isEnabled()).toBe(false);
        expect(tagsPage.getCombineButton().isEnabled()).toBe(true);
        expect(tagsPage.getClearButton().isEnabled()).toBe(true);
    });

    it('should add a tag', async () => {
        const tagName = 'Test Tag 1';

        await appPage.setOrganizeMode(true);
        await tagsPage.getAddButton().click();
        E2eUtil.browserWaitFor(tagsPage.addModalCss);
        await inputDialog.getInputBox().sendKeys(tagName);
        await inputDialog.getOkButton().click();
        const tags = await tagsPage.getTagNames(false);

        expect(tags.length).toBeGreaterThan(0);
        expect(tags).toContain(tagName);
    });

    it('should copy tag', async () => {
        const tagName = 'Test Tag 1';
        const copiedTagName = 'Test Tag 1 Copied';

        await appPage.setOrganizeMode(true);
        await tagsPage.selectTag(tagName);
        await tagsPage.getCopyButton().click();
        E2eUtil.browserWaitFor(tagsPage.copyModalCss);
        await inputDialog.getInputBox().clear();
        await inputDialog.getInputBox().sendKeys(copiedTagName);
        await inputDialog.getOkButton().click();
        const tags = await tagsPage.getTagNames(false);

        expect(tags.length).toBeGreaterThan(0);
        expect(tags).toContain(copiedTagName);
    });

    it('should combine tags', async () => {
        const tagName = 'Test Tag 1';
        const copiedTagName = 'Test Tag 1 Copied';

        await appPage.setOrganizeMode(true);
        await tagsPage.selectTag(tagName);
        await tagsPage.selectTag(copiedTagName);
        await tagsPage.getCombineButton().click();
        E2eUtil.browserWaitFor(tagsPage.combineModalCss);
        await inputDialog.getInputBox().clear();
        await inputDialog.getInputBox().sendKeys(tagName);
        await inputDialog.getOkButton().click();
        const tags = await tagsPage.getTagNames(false);

        expect(tags.length).toBeGreaterThan(0);
        expect(tags).toContain(tagName);
        expect(tags).not.toContain(copiedTagName);
    });

    it('should rename a tag', async () => {
        const tagName = 'Test Tag 1';
        const renamedTagName = 'Test Tag 1 Renamed';

        await appPage.setOrganizeMode(true);
        await tagsPage.selectTag(tagName);
        await tagsPage.getRenameButton().click();
        E2eUtil.browserWaitFor(tagsPage.renameModalCss);
        await inputDialog.getInputBox().clear();
        await inputDialog.getInputBox().sendKeys(renamedTagName);
        await inputDialog.getOkButton().click();
        const tags = await tagsPage.getTagNames(false);

        expect(tags.length).toBeGreaterThan(0);
        expect(tags).toContain(renamedTagName);
    });

    it('should delete tags', async () => {
        const renamedTagName = 'Test Tag 1 Renamed';

        await appPage.setOrganizeMode(true);
        await tagsPage.selectTag(renamedTagName);
        await tagsPage.getDeleteButton().click();
        E2eUtil.browserWaitFor(tagsPage.deleteModalCss);
        await confirmDialog.getYesButton().click();
        const tags = await tagsPage.getTagNames(false);

        expect(tags.length).toBeGreaterThan(0);
        expect(tags).not.toContain(renamedTagName);
    });
});
