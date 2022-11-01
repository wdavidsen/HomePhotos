import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { User } from '../models';
import { AuthService } from '../services';

import { LoginComponent } from './login.component';

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;
  let mockToastr, mockAuthenticationService, mockModalService, mockModalRef;

  const setupForm = (data) => {
    const formBuilder = TestBed.get(UntypedFormBuilder);
    component.loginForm = formBuilder.group({
      username: [data.username, Validators.required],
      password: [data.password, Validators.required]
    });
  };

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockAuthenticationService = jasmine.createSpyObj(['currentUserValue', 'login', 'logout', 'loadCsrfToken']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);
    mockModalRef = jasmine.createSpyObj(['hide']);

    TestBed.configureTestingModule({
      declarations: [ LoginComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: ToastrService, useValue: mockToastr },
        { provide: BsModalService, useValue: mockModalService },
        { provide: BsModalRef, useValue: mockModalRef}
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.loginForm).toBeTruthy();
  });

  it('should save form', () => {
    setupForm( {username: 'wdavidsen', password: 'password1'} );
    mockAuthenticationService.login.and.returnValue(of(new User()));

    component.onSubmit();

    expect(mockAuthenticationService.login).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should not save incomplete form', () => {
    setupForm( {username: 'wdavidsen', password: null} );
    mockAuthenticationService.login.and.returnValue(of(new User()));

    component.onSubmit();

    expect(mockAuthenticationService.login).toHaveBeenCalledTimes(0);
    expect(mockToastr.success).toHaveBeenCalledTimes(0);
  });

  it('should handle save form error', () => {
    setupForm( {username: 'wdavidsen', password: 'password1'} );
    const sub = new Subject<User>();
    mockAuthenticationService.login.and.returnValue(sub.asObservable());

    sub.error('some error');
    component.onSubmit();

    expect(mockAuthenticationService.login).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });
});
