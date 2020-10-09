import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ModalModule } from 'ngx-bootstrap/modal';
import { IndividualConfig, ToastrService } from 'ngx-toastr';
import { SettingsService } from '../services/settings.service';

import { SettingsComponent } from './settings.component';

describe('SettingsComponent', () => {
  let component: SettingsComponent;
  let fixture: ComponentFixture<SettingsComponent>;
  let mockToastr, mockSettingsService;

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockSettingsService = jasmine.createSpyObj(['getSettings', 'updateSettings', 'indexNow', 'clearCache']);

    TestBed.configureTestingModule({
      declarations: [ SettingsComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule, ModalModule, FormsModule, ModalModule.forRoot()],
      providers: [
        SettingsService,
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
});
