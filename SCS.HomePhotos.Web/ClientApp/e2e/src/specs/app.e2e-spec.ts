import { AppPage } from '../pages/app.po';

describe('App', () => {
  let page: AppPage;

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display title', () => {
    page.navigateTo();
    expect(page.getTitle()).toEqual('ePhotoBox');
  });
});
