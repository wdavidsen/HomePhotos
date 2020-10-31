import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { IndividualConfig, ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { AccountInfo } from '../models';
import { AccountService, AuthService } from '../services';

import { AccountComponent } from './account.component';

describe('AccountComponent', () => {
  let component: AccountComponent;
  let fixture: ComponentFixture<AccountComponent>;
  let mockToastr, mockAuthenticationService, mockAccountService, mockModalService;

  const currentUser = {userId: 1, username: 'wdavidsen', firstName: 'Bill', lastName: 'Davidsen'};

  const setupEditForm = (data) => {
    const formBuilder = TestBed.get(FormBuilder);
      component.accountForm = formBuilder.group({
        username: [data.username, Validators.required],
        firstName: [data.firstName, Validators.required],
        lastName: [data.lastName, Validators.required]
    });
  };

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockAuthenticationService = jasmine.createSpyObj(['getCurrentUser']);
    mockAccountService = jasmine.createSpyObj(['save']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);

    mockAuthenticationService.getCurrentUser.and.returnValue(of(currentUser));

    TestBed.configureTestingModule({
      declarations: [ AccountComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      providers: [
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: AccountService, useValue: mockAccountService },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.accountInfo).toBeTruthy();
    expect(component.accountForm).toBeTruthy();
    expect(mockAuthenticationService.getCurrentUser).toHaveBeenCalledTimes(1);
  });

  it('should show change password modal', () => {

    component.changePassword();

    expect(mockModalService.show).toHaveBeenCalledTimes(1);
  });

  it('should save form', () => {
    setupEditForm(currentUser);
    mockAccountService.save.and.returnValue(of(currentUser));

    component.onSubmit();

    expect(mockAccountService.save).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should not save incomplete form', () => {
    const incompleteUser = {userId: 1, username: 'wdavidsen', firstName: null, lastName: null };
    setupEditForm(incompleteUser);
    mockAccountService.save.and.returnValue(of(currentUser));

    component.onSubmit();

    expect(mockAccountService.save).toHaveBeenCalledTimes(0);
    expect(mockToastr.success).toHaveBeenCalledTimes(0);
  });

  it('should handle save form error', () => {
    const sub = new Subject<AccountInfo>();
    mockAccountService.save.and.returnValue(sub.asObservable());

    sub.error('some error');
    component.onSubmit();

    expect(mockAccountService.save).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });
});
