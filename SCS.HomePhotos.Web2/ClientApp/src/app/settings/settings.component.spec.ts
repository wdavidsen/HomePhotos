import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import * as moment from 'moment';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { Settings } from '../models';
import { SettingsService } from '../services/settings.service';

import { SettingsComponent } from './settings.component';

describe('SettingsComponent', () => {
  let component: SettingsComponent;
  let fixture: ComponentFixture<SettingsComponent>;
  let mockToastr, mockSettingsService, mockModalService;

  const settings: Settings = {
    cacheFolder: 'cache-folder',
    mobileUploadsFolder: 'mobile-folder',
    indexPath: 'index-path',
    nextIndexTime: new Date(),
    indexFrequencyHours: 24,
    smallImageSize: 640,
    largeImageSize: 1248,
    thumbnailSize: 250
  };

  const setupEditForm = (data) => {
    const formBuilder = TestBed.get(UntypedFormBuilder);
    component.settingsForm = formBuilder.group({
      indexPath: [data.indexPath, Validators.required],
      cacheFolder: [data.cacheFolder, Validators.required],
      mobileUploadsFolder: [data.mobileUploadsFolder, Validators.required],
      nextIndexTime_date: [moment(data.nextIndexTime).format('YYYY-MM-DD')],
      nextIndexTime_time: [moment(data.nextIndexTime).format('HH:mm')],
      indexFrequencyHours: [data.indexFrequencyHours, Validators.required],
      largeImageSize: [data.largeImageSize, Validators.required],
      smallImageSize: [data.smallImageSize, Validators.required],
      thumbnailSize: [data.thumbnailSize, Validators.required]});
  };

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockSettingsService = jasmine.createSpyObj(['getSettings', 'updateSettings', 'indexNow', 'clearCache']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);

    mockSettingsService.getSettings.and.returnValue(of(settings));

    TestBed.configureTestingModule({
      declarations: [ SettingsComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule, ModalModule, FormsModule, ModalModule.forRoot()],
      providers: [
        { provide: SettingsService, useValue: mockSettingsService },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.settings).toBeTruthy();
    expect(component.settingsForm).toBeTruthy();
    expect(mockSettingsService.getSettings).toHaveBeenCalledTimes(1);
  });

  it('should clear cache', () => {
    const sub = new Subject<any>();
    mockSettingsService.clearCache.and.returnValue(sub.asObservable());

    component.clear();

    expect(mockSettingsService.clearCache).toHaveBeenCalledTimes(1);

    sub.next();
    fixture.detectChanges();

    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should handle clear cache error', () => {
    const sub = new Subject<any>();
    mockSettingsService.clearCache.and.returnValue(sub.asObservable());

    component.clear();

    expect(mockSettingsService.clearCache).toHaveBeenCalledTimes(1);

    sub.error(null);
    fixture.detectChanges();

    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });

  // it('should display clear cache modal', () => {
  //   const modalService = TestBed.get(BsModalService);
  //   const sub = new Subject<any>();
  //   modalService.onHidden = sub;

  //   component.promptForClear();

  //   expect(mockModalService.show).toHaveBeenCalledTimes(1);
  // });

  it('should index now', () => {
    const sub = new Subject<Settings>();
    mockSettingsService.indexNow.and.returnValue(sub.asObservable());

    component.index();

    expect(mockSettingsService.indexNow).toHaveBeenCalledTimes(1);

    sub.next(settings);
    fixture.detectChanges();

    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should handle index now error', () => {
    const sub = new Subject<Settings>();
    mockSettingsService.indexNow.and.returnValue(sub.asObservable());

    component.index();

    expect(mockSettingsService.indexNow).toHaveBeenCalledTimes(1);

    sub.error(new Error('some error'));
    fixture.detectChanges();

    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });

  it('should save form', () => {
    setupEditForm(settings);
    mockSettingsService.updateSettings.and.returnValue(of(settings));

    component.onSubmit();

    expect(mockSettingsService.updateSettings).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should not save incomplete form', () => {
    const incompleteSettings = {...settings};
    incompleteSettings.thumbnailSize = null;

    setupEditForm(incompleteSettings);
    mockSettingsService.updateSettings.and.returnValue(of(settings));

    component.onSubmit();

    expect(mockSettingsService.updateSettings).toHaveBeenCalledTimes(0);
    expect(mockToastr.success).toHaveBeenCalledTimes(0);
  });

  it('should handle save form error', () => {
    const sub = new Subject<Settings>();
    mockSettingsService.updateSettings.and.returnValue(sub.asObservable());

    sub.error('some error');
    component.onSubmit();

    expect(mockSettingsService.updateSettings).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });
});
