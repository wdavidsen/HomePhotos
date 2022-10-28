import { HttpClientModule } from '@angular/common/http';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { AuthService } from '../services';
import { UploadComponent } from './upload.component';

describe('UploadComponent', () => {
  let component: UploadComponent;
  let fixture: ComponentFixture<UploadComponent>;
  let mockToastr, mockAuthenticationService, mockModalService;

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockAuthenticationService = jasmine.createSpyObj(['currentUserValue', 'login', 'getCurrentUser', 'getJwtToken', 'loadCsrfToken']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);

    mockAuthenticationService.getCurrentUser.and.returnValue(of({username: 'wdavidsen'}));
    mockAuthenticationService.loadCsrfToken.and.returnValue(of(true));

    TestBed.configureTestingModule({
      declarations: [ UploadComponent ],
      imports: [HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      providers: [
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.uploader).toBeTruthy();
  });
});
