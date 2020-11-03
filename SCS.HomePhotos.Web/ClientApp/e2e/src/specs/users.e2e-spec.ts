import { assert } from 'console';
import { browser } from 'protractor';
import { E2eUtil } from '../e2e-util';
import { LoginPage } from '../pages/login.po';
import { PhotosPage } from '../pages/photos.po';
import { UserDetailPage } from '../pages/user-detail.po';
import { UsersPage } from '../pages/users.po';

describe('Users', () => {
    const usersPage = new UsersPage();
    const userDetailPage = new UserDetailPage();
    const photosPage = new PhotosPage();

    beforeEach(async () => {
        const loginPage = new LoginPage();
        loginPage.navigateTo();
        await E2eUtil.browserWaitFor(loginPage.loginButtonCss);
        await loginPage.login('wdavidsen', 'Pass@123');
        await E2eUtil.browserWaitFor(photosPage.photosCss);
        await usersPage.navigateTo();
        await E2eUtil.browserWaitFor(usersPage.toolbarCss);
    });

    xit('should display users', async () => {
        const rowData = await usersPage.getRowData(0);

        expect(rowData.userId).toBeTruthy();
        expect(rowData.firstName).toBeTruthy();
        expect(rowData.lastName).toBeTruthy();
        expect(rowData.lastLogin).toBeTruthy();
        expect(rowData.failedLogins).toBeGreaterThan(-1);
        expect(rowData.enabled).toBeTruthy();
        expect(rowData.role).toBeTruthy();
    });

    xit('should enable/disable toolbar buttons', async () => {
        expect(usersPage.getNewButton().isEnabled()).toBe(true);
        expect(usersPage.getEditButton().isEnabled()).toBe(false);
        expect(usersPage.getDeleteButton().isEnabled()).toBe(false);
        expect(usersPage.getEnableButton().isEnabled()).toBe(false);
        expect(usersPage.getDisableButton().isEnabled()).toBe(false);

        await usersPage.clickRow(0);

        expect(usersPage.getNewButton().isEnabled()).toBe(true);
        expect(usersPage.getEditButton().isEnabled()).toBe(true);
        expect(usersPage.getDeleteButton().isEnabled()).toBe(true);
        expect(usersPage.getEnableButton().isEnabled()).toBe(true);
        expect(usersPage.getDisableButton().isEnabled()).toBe(true);
    });

    xit('should navigate to user details', async () => {
        await usersPage.clickRow(0);
        await usersPage.getEditButton().click();

        E2eUtil.browserWaitFor(userDetailPage.componentCss);
    });

    it('should add a user', async () => {
        await usersPage.getNewButton().click();
        E2eUtil.browserWaitFor(userDetailPage.componentCss);
        expect(await userDetailPage.getTitle().getText()).toBe('New User');

        await userDetailPage.getUserIdBox().sendKeys('testuser1');
        await userDetailPage.getPasswordBox().sendKeys('Pass@1111');
        await userDetailPage.getPasswordCompareBox().sendKeys('Pass@1111');
        await userDetailPage.getFirstNameBox().sendKeys('FirstNameTest1');
        await userDetailPage.getLastNameBox().sendKeys('LastNameTest1');
        await userDetailPage.selectRole('Contributer');
        await userDetailPage.getSaveButton().click();

        E2eUtil.browserWaitFor(userDetailPage.summaryInfoCss);
        expect(await userDetailPage.getTitle().getText()).toBe('FirstNameTest1 LastNameTest1');
    });

    it('should enable/disable a user', async () => {
        let rowIndex = await usersPage.getUserRowIndex('testuser1');
        expect(rowIndex).toBeGreaterThan(-1);
        await usersPage.clickRow(rowIndex);
        await usersPage.getDisableButton().click();

        rowIndex = await usersPage.getUserRowIndex('testuser1');
        expect(rowIndex).toBeGreaterThan(-1);
        let rowData = await usersPage.getRowData(rowIndex);
        expect(rowData.enabled).toBe('No');

        await usersPage.getEnableButton().click();
        rowData = await usersPage.getRowData(rowIndex);
        expect(rowData.enabled).toBe('Yes');
    });

    it('should edit a user', async () => {
        let rowIndex = await usersPage.getUserRowIndex('testuser1');
        expect(rowIndex).toBeGreaterThan(-1);
        await usersPage.clickRow(rowIndex);
        await usersPage.getEditButton().click();

        E2eUtil.browserWaitFor(userDetailPage.componentCss);
        expect(await userDetailPage.getTitle().getText()).toBe('FirstNameTest1 LastNameTest1');
        expect(await userDetailPage.getSummaryInfo().getText()).toContain('Failed login count: 0');
        expect(await userDetailPage.getUserIdBox().getAttribute('value')).toBe('testuser1');
        expect(await userDetailPage.getFirstNameBox().getAttribute('value')).toBe('FirstNameTest1');
        expect(await userDetailPage.getLastNameBox().getAttribute('value')).toBe('LastNameTest1');

        await userDetailPage.selectRole('Admin');
        await userDetailPage.getSaveButton().click();
        await userDetailPage.getReturnLink().click();
        E2eUtil.browserWaitFor(usersPage.componentCss);

        rowIndex = await usersPage.getUserRowIndex('testuser1');
        const rowData = await usersPage.getRowData(rowIndex);
        expect(rowData.role).toBe('Admin');
    });

    it('should delete a user from list', async () => {
        let rowIndex = await usersPage.getUserRowIndex('testuser1');
        await usersPage.clickRow(rowIndex);
        await usersPage.getDeleteButton().click();
        await E2eUtil.acceptConfirm();

        rowIndex = await usersPage.getUserRowIndex('testuser1');
        expect(rowIndex).toBe(-1);
    });

    it('should delete a user from detail view', async () => {
        await usersPage.getNewButton().click();

        E2eUtil.browserWaitFor(userDetailPage.componentCss);
        expect(await userDetailPage.getTitle().getText()).toBe('New User');

        await userDetailPage.getUserIdBox().sendKeys('testuser2');
        await userDetailPage.getPasswordBox().sendKeys('Pass@22222');
        await userDetailPage.getPasswordCompareBox().sendKeys('Pass@22222');
        await userDetailPage.getFirstNameBox().sendKeys('FirstNameTest2');
        await userDetailPage.getLastNameBox().sendKeys('LastNameTest2');
        await userDetailPage.getSaveButton().click();

        E2eUtil.browserWaitFor(userDetailPage.summaryInfoCss);
        await userDetailPage.getDeleteButton().click();

        E2eUtil.browserWaitFor(usersPage.componentCss);
        const rowIndex = await usersPage.getUserRowIndex('testuser2');
        expect(rowIndex).toBe(-1);
    });
});
