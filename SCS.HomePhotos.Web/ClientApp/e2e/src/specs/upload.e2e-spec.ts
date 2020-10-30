import { browser } from 'protractor';
import { E2eUtil } from '../e2e-util';
import { FileSelectDialog } from '../pages/dialogs/file-select-dialog.po';
import { UploadTaggerDialog } from '../pages/dialogs/upload-tagger-dialog.po';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';
import { UploadPage } from '../pages/upload.po';

describe('Tags', () => {
    const uploadPage = new UploadPage();
    const photosPage = new PhotosPage();
    const fileSelectDialog = new FileSelectDialog();
    const uploadTaggerDialog = new UploadTaggerDialog();

    beforeEach(async () => {
        const loginPage = new LoginPage();
        loginPage.navigateTo();
        await E2eUtil.browserWaitFor(loginPage.loginButtonCss);
        await loginPage.login('wdavidsen', 'Pass@123');
        await E2eUtil.browserWaitFor(photosPage.photosCss);
        await uploadPage.navigateTo();
        await E2eUtil.browserWaitFor(uploadPage.componentCss);
    });

    it('should upload image', async () => {
        await uploadPage.getSelectButton().click();
        E2eUtil.browserWaitFor(fileSelectDialog.fileInputCss);
        await fileSelectDialog.getFileInputBox().sendKeys('c:\\temp\\whale shark.png');
        E2eUtil.browserWaitFor(uploadPage.fileTilesCss);

        await uploadPage.getUploadButton().click();
        E2eUtil.browserWaitFor(uploadTaggerDialog.inputCss);
        await uploadTaggerDialog.getInputBox().sendKeys('Test Whale Shark');
        await uploadTaggerDialog.getOkButton().click();
        E2eUtil.browserWaitFor(uploadPage.getImageSuccessCss(0));

        expect(await uploadPage.getImageStatus(0)).toContain('Uploaded');
    });
});
