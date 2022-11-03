import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { UntypedFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { Observable, of, Subject } from 'rxjs';
import { ActivatedRouteStub } from 'test/activated-route-stub';
import { User } from '../models';
import { AuthService, UserService } from '../services';
import { UserDetailComponent } from './user-detail.component';

describe('UserDetailComponent', () => {
  let component: UserDetailComponent;
  let fixture: ComponentFixture<UserDetailComponent>;
  let mockToastr, mockAuthenticationService, mockUserService, mockModalService, mockActivatedRoute, mockRouter;

  const user: User = { userId: 1, username: 'wdavidsen', password: 'password1', passwordCompare: 'password1', firstName: 'Bill', lastName: 'Davidsen', emailAddress: 'wdavidsen@gmail.com',
    role: 'Admin', lastLogin: new Date(), failedLoginCount: 0, mustChangePassword: false, enabled: true, jwt: null, refreshToken: null, avatarImage: null};

  const setupEditForm = (data) => {
    const formBuilder = TestBed.get(UntypedFormBuilder);
      component.userForm = formBuilder.group({
        username: [data.username, Validators.required],
        password: [data.password, Validators.required],
        passwordCompare: [data.passwordCompare, Validators.required],
        firstName: [data.firstName, Validators.required],
        lastName: [data.lastName, Validators.required],
        emailAddress: [data.emailAddress, Validators.email],
        role: [data.role, Validators.required],
        lastLogin: [data.lastLogin],
        failedLoginCount: [data.failedLoginCount],
        mustChangePassword: [data.mustChangePassword],
        enabled: [data.enabled]
    });
  };

  class RouterStub {
    constructor() {
        this.events = new Observable<Event>();
    }
    events: Observable<Event>;
  }

  beforeEach(async(() => {
    mockActivatedRoute = new ActivatedRouteStub({ userId: null });

    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockAuthenticationService = jasmine.createSpyObj(['currentUserValue', 'login', 'getCurrentUser']);
    mockUserService = jasmine.createSpyObj(['get', 'delete', 'save']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);
    mockRouter = new RouterStub();

    mockAuthenticationService.getCurrentUser.and.returnValue(of({username: 'jdoe', role: 'Admin'}));
    mockUserService.get.and.returnValue(of(user));

    TestBed.configureTestingModule({
      declarations: [ UserDetailComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: UserService, useValue: mockUserService},
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize new form', () => {
    expect(component.user).toBeTruthy();
    expect(component.userForm).toBeTruthy();
    expect(component.user.userId).toBeNull();
    expect(component.user.username).toBeNull();
    expect(component.user.firstName).toBeNull();
    expect(component.user.lastName).toBeNull();
    expect(component.user.role).toBe('Reader');
    expect(component.user.mustChangePassword).toBe(true);
    expect(component.user.enabled).toBe(true);
  });

  it('should initialize edit form', () => {
    const userId = 1;
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: userId });

    expect(component.user).toBeTruthy();
    expect(component.userForm).toBeTruthy();
    expect(component.user.userId).toBe(userId);
    expect(component.user.username).toBe('wdavidsen');
    expect(component.user.firstName).toBe('Bill');
    expect(component.user.lastName).toBe('Davidsen');
    expect(component.user.role).toBe('Admin');
    expect(component.user.mustChangePassword).toBe(false);
    expect(component.user.enabled).toBe(true);
  });

  it('should save form', () => {
    setupEditForm(user);
    mockUserService.save.and.returnValue(of(user));

    component.onSubmit();

    expect(component.userForm.valid).toBe(true);
    expect(mockUserService.save).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
    expect(component.loading).toBeFalsy();
  });

  it('should not save incomplete form', () => {
    const u = {...user};
    u.firstName = null;

    setupEditForm(u);
    mockUserService.save.and.returnValue(of(u));

    component.onSubmit();

    expect(component.userForm.valid).toBe(false);
    expect(mockUserService.save).toHaveBeenCalledTimes(0);
    expect(mockToastr.success).toHaveBeenCalledTimes(0);
  });

  it('should handle save form error', () => {
    setupEditForm(user);
    const sub = new Subject<User>();
    mockUserService.save.and.returnValue(sub.asObservable());

    sub.error('some error');
    component.onSubmit();

    expect(component.userForm.valid).toBe(true);
    expect(mockUserService.save).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });

  it('should delete user', () => {
    const userId = 1;
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: userId });

    mockUserService.delete.and.returnValue(of(null));

    component.deleteUser();

    expect(mockUserService.delete).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
    expect(component.loading).toBeFalsy();
  });

  it('should handle delete user error', () => {
    const userId = 1;
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: userId });

    const sub = new Subject<any>();
    mockUserService.delete.and.returnValue(sub);

    sub.error('some error');
    component.deleteUser();

    expect(mockUserService.delete).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
    expect(component.loading).toBeFalsy();
  });

  it('should not allow current user to be deleted', () => {
    const userId = 1;
    const mockUser = {...user};
    mockUser.username = 'jdoe';
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: userId });

    const sub = new Subject<any>();
    mockUserService.delete.and.returnValue(sub);

    sub.error('some error');
    component.deleteUser();

    expect(mockUserService.delete).toHaveBeenCalledTimes(1);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
    expect(component.loading).toBeFalsy();
  });

  it('should render form for new user', () => {
    expect(fixture.nativeElement.querySelectorAll('.form-group input[type=text]').length).toBe(4);
    expect(fixture.nativeElement.querySelectorAll('.form-group input[type=password]').length).toBe(2);
    expect(fixture.nativeElement.querySelectorAll('.form-group select').length).toBe(1);
  });

  it('should render form for existing user', () => {
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: user.userId });
    component.ngOnInit();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.form-group input[type=text]').length).toBe(4);
    expect(fixture.nativeElement.querySelectorAll('.form-group input[type=password]').length).toBe(0);
    expect(fixture.nativeElement.querySelectorAll('.form-group select').length).toBe(1);
  });

  it('should bind form data for existing user', () => {
    setupEditForm(user);
    mockActivatedRoute.setParamMap({ userId: user.userId });
    component.ngOnInit();
    fixture.detectChanges();

    const roleSelect = fixture.nativeElement.querySelector('#roleSelect');

    expect(fixture.nativeElement.querySelector('#usernameInput').value).toBe(user.username);
    expect(fixture.nativeElement.querySelector('#firstNameInput').value).toBe(user.firstName);
    expect(fixture.nativeElement.querySelector('#lastNameInput').value).toBe(user.lastName);
    expect(roleSelect.options[roleSelect.selectedIndex].text).toBe(user.role);
    expect(fixture.nativeElement.querySelector('#enabledCheck').checked).toBe(true);
    expect(fixture.nativeElement.querySelector('#mustChangePasswordCheck').checked).toBe(false);
  });
});
