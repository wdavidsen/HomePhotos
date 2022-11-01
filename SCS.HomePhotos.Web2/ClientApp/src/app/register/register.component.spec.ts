import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { AccountService, AuthService } from '../services';

import { RegisterComponent } from './register.component';

describe('RegisterComponent', () => {
  let component: RegisterComponent;
  let fixture: ComponentFixture<RegisterComponent>;
  let mockToastr, mockAuthenticationService, mockAccountService;

  const userInfo = {userId: 1, username: 'wdavidsen', firstName: 'Bill', lastName: 'Davidsen', password: 'password1', passwordCompare: 'password1', role: 'Contributer' };

  const setupEditForm = (data) => {
    const formBuilder = TestBed.get(UntypedFormBuilder);
    component.registerForm = formBuilder.group({
        firstName: [data.firstName, Validators.required],
        lastName: [data.lastName, Validators.required],
        username: [data.username, Validators.required],
        password: [data.password, [Validators.required, Validators.minLength(8)]],
        passwordCompare: [data.passwordCompare, [Validators.required, Validators.minLength(8)]]
    });
  };

  beforeEach(async(() => {
    mockAuthenticationService = jasmine.createSpyObj(['getCurrentUser', 'loadCsrfToken']);
    mockAccountService = jasmine.createSpyObj(['register']);
    mockToastr = jasmine.createSpyObj(['success', 'error']);

    mockAuthenticationService.loadCsrfToken.and.returnValue(of(true));

    TestBed.configureTestingModule({
      declarations: [ RegisterComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: AccountService, useValue: mockAccountService },
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.registerForm).toBeTruthy();
  });

  it('should save form', () => {
    setupEditForm(userInfo);
    mockAccountService.register.and.returnValue(of({userInfo}));

    component.onSubmit();

    expect(mockAccountService.register).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should not save incomplete form', () => {
    const incompleteUser = {userId: 1, username: 'wdavidsen', firstName: null, lastName: null };
    setupEditForm(incompleteUser);
    mockAccountService.register.and.returnValue(of(userInfo));

    component.onSubmit();

    expect(mockAccountService.register).toHaveBeenCalledTimes(0);
    expect(mockToastr.success).toHaveBeenCalledTimes(0);
  });

  it('should handle save form error', () => {
    setupEditForm(userInfo);
    const sub = new Subject<any>();
    mockAccountService.register.and.returnValue(sub.asObservable());

    sub.error('some error');
    component.onSubmit();

    expect(mockAccountService.register).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });
});
