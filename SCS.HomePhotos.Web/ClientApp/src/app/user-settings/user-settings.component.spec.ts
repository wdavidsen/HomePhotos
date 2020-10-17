import { HttpClientModule } from '@angular/common/http';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalRef, ModalModule } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { UserSettings } from '../models/user-settings';
import { UserSettingsService } from '../services';
import { UserSettingsComponent } from './user-settings.component';

describe('UserSettingsComponent', () => {
  let component: UserSettingsComponent;
  let fixture: ComponentFixture<UserSettingsComponent>;
  let mockToastr, mockUserSettingsService, mockModalRef;

  const userSettings: UserSettings = {
    thumbnailSize: 'Large',
    slideshowSpeed: 'Fast',
    autoStartSlideshow: false
  };

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockModalRef = jasmine.createSpyObj(['hide']);
    mockUserSettingsService = jasmine.createSpyObj(['getSettings', 'saveSettings']);

    mockUserSettingsService.getSettings.and.returnValue(of({...userSettings}));

    TestBed.configureTestingModule({
      declarations: [ UserSettingsComponent ],
      imports: [HttpClientModule, RouterTestingModule, ReactiveFormsModule, ModalModule.forRoot()],
      providers: [
        { provide: UserSettingsService, useValue: mockUserSettingsService },
        { provide: BsModalRef, useValue: mockModalRef},
        { provide: ToastrService, useValue: mockToastr }
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserSettingsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.userSettings).toBeTruthy();
    expect(component.userSettingsForm).toBeTruthy();
  });

  it('should save', () => {
    component.onSubmit();

    expect(mockUserSettingsService.saveSettings).toHaveBeenCalledTimes(1);
  });

  it('should not save invalid form', () => {
    component.userSettings.thumbnailSize = null;
    fixture.detectChanges();

    component.onSubmit();

    expect(mockUserSettingsService.saveSettings).toHaveBeenCalledTimes(1);
  });
});
