import { $, browser, promise, protractor } from 'protractor';

export class E2eUtil {
    static EC = protractor.ExpectedConditions;

    static browserWaitFor(css: string): promise.Promise<unknown> {
        return browser.wait(this.EC.presenceOf($(css)), 7000);
    }

    static async acceptConfirm() {
        await (browser.switchTo().alert()).accept();
    }

    static async dismissConfirm() {
        await (browser.switchTo().alert()).dismiss();
    }
}
