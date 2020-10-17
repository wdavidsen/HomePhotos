import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthenticationService, PhotosService, SearchService, UserSettingsService } from '../services';

import { PhotosComponent } from './photos.component';
import { LocalStorageService } from '../services/local-storage.service';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { of } from 'rxjs';
import { UserSettings } from '../models/user-settings';
import { Photo } from '../models';
import { ActivatedRoute, Router, Routes } from '@angular/router';
import { ActivatedRouteStub } from 'test/activated-route-stub';

describe('PhotosComponent', () => {
  let component: PhotosComponent;
  let fixture: ComponentFixture<PhotosComponent>;
  let mockToastr, mockPhotoService, mockAuthenticationService, mockUserSettingsService, mockActivatedRoute, mockModalService;
  let router: Router;

  const photoList: Photo[] = [
    { photoId: 1, dateTaken: new Date(), cacheFolder: 'ab', fileName: 'file1.jpg', imageWidth: 120, imageHeight: 90 },
    { photoId: 2, dateTaken: new Date(), cacheFolder: 'ab', fileName: 'file2.jpg', imageWidth: 90, imageHeight: 120 }
  ];

  const settings = new UserSettings();
  const routes: Routes = [
    { path: '', component: PhotosComponent },
    { path: 'photos/:tagName', component: PhotosComponent }
  ];

  beforeEach(async(() => {
    mockActivatedRoute = new ActivatedRouteStub({ tagName: null });

    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockPhotoService = jasmine.createSpyObj(['getLatest', 'getPhotosByTag', 'searchPhotos']);
    mockAuthenticationService = jasmine.createSpyObj(['getCurrentUser']);
    mockUserSettingsService = jasmine.createSpyObj(['userSettings', 'getSettings']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);

    mockPhotoService.getLatest.and.returnValue(of(photoList));
    mockPhotoService.getPhotosByTag.and.returnValue(of(photoList));
    mockPhotoService.searchPhotos.and.returnValue(of(photoList));
    mockAuthenticationService.getCurrentUser.and.returnValue(of({userId: 1, username: 'wdavidsen'}));
    mockUserSettingsService.userSettings.and.returnValue(settings);
    mockUserSettingsService.getSettings.and.returnValue(of(settings));

    TestBed.configureTestingModule({
      declarations: [ PhotosComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule.withRoutes(routes), ModalModule.forRoot()],
      providers: [
        LocalStorageService,
        SearchService,
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: AuthenticationService, useValue: mockAuthenticationService },
        { provide: UserSettingsService, useValue: mockUserSettingsService },
        { provide: PhotosService, useValue: mockPhotoService },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotosComponent);
    component = fixture.componentInstance;

    router = TestBed.get(Router);
    // location = TestBed.get(Location);

    fixture.detectChanges();
    router.initialNavigation();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with no route params', () => {
    expect(component.thumbnails.length).toBeTruthy();
    expect(component.organizeMode).toBeFalsy();
    expect(component.tagName).toBeNull();
    // expect(component.taggerModalRef).toBeTruthy();
  });

  it('should load latest photos with no route params', () => {
    expect(mockPhotoService.getLatest).toHaveBeenCalledTimes(1);
    expect(component.thumbnails[0].photoId).toBe(1);
    expect(component.thumbnails[0].selected).toBe(false);
    expect(component.thumbnails[0].thumbHeight).toBe(150);
    expect(component.thumbnails[0].thumbWidth).toBe(200);
    expect(component.thumbnails[0].thumbUrl).toBe('/photo-image/ab/file1.jpg');
  });

  it('should initialize with tag route param', () => {
    const tag = 'birthday';
    mockActivatedRoute.setParamMap({ tagName: tag });

    mockPhotoService.getPhotosByTag.calls.reset();
    component.ngOnInit();

    expect(component.tagName).toBe(tag);
    expect(component.organizeMode).toBeFalsy();
  });

  it('should load photos by tag', () => {
    const tag = 'birthday';
    mockActivatedRoute.setParamMap({ tagName: tag });

    mockPhotoService.getPhotosByTag.calls.reset();
    component.ngOnInit();

    expect(mockPhotoService.getPhotosByTag).toHaveBeenCalledTimes(1);
    expect(component.thumbnails[0].photoId).toBe(1);
    expect(component.thumbnails[0].selected).toBe(false);
    expect(component.thumbnails[0].thumbHeight).toBe(150);
    expect(component.thumbnails[0].thumbWidth).toBe(200);
    expect(component.thumbnails[0].thumbUrl).toBe('/photo-image/ab/file1.jpg');
  });

  it('should search photos by keyword', () => {
    const keyword = 'party';

    const searchService: SearchService = TestBed.get(SearchService);
    searchService.setKeywords(keyword);

    expect(mockPhotoService.searchPhotos).toHaveBeenCalledTimes(1);
    expect(component.keywords).toBe(keyword);
  });

  it('should clear search and get latest photos', () => {
    mockPhotoService.getLatest.calls.reset();
    const searchService: SearchService = TestBed.get(SearchService);

    component.keywords = 'party';
    searchService.setKeywords(null);

    expect(mockPhotoService.getLatest).toHaveBeenCalledTimes(1);
    expect(component.keywords).toBeNull();
  });

  it('should render thumbnails', () => {
    const firstThumb = component.thumbnails[0];

    expect(fixture.nativeElement.querySelectorAll('.photo-list div').length).toBe(2);

    const style = fixture.nativeElement.querySelector('.photo-list div:first-child').getAttribute('style');
    expect(style).toContain(`url("${firstThumb.thumbUrl}")`);
    expect(style).toContain(`height: ${firstThumb.thumbHeight}px`);
    expect(style).toContain(`width: ${firstThumb.thumbWidth}px`);
  });

  it('should select all photos', () => {
    component.selectAll();

    const selectedThumbs = component.thumbnails.filter(thumb => thumb.selected);

    expect(component.thumbnails.length).toBeGreaterThan(0);
    expect(selectedThumbs.length).toBe(2);
  });

  it('should get selected photos', () => {
    component.thumbnails[1].selected = true;

    const selections = component.getSelectedThumbnails();

    expect(selections.length).toBe(1);
    expect(selections[0]).toEqual(component.thumbnails[1]);
  });

  it('should clear all selected photos', () => {
    component.thumbnails[0].selected = true;
    component.thumbnails[1].selected = true;

    component.clearSelections();

    const selectedThumbs = component.thumbnails.filter(thumb => thumb.selected);

    expect(component.thumbnails.length).toBeGreaterThan(0);
    expect(selectedThumbs.length).toBe(0);
  });

  it('should display tag modal', () => {
    component.thumbnails[0].selected = true;

    component.showTagTool();

    expect(mockModalService.show).toHaveBeenCalledTimes(1);
  });
});
