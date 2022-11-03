import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { ToastrService } from 'ngx-toastr';
import { Observable, of, Subject } from 'rxjs';
import { AuthService, UserService } from '../services';
import { UsersComponent } from './users.component';

describe('UsersComponent', () => {
  let component: UsersComponent;
  let fixture: ComponentFixture<UsersComponent>;
  let mockToastr, mockAuthenticationService, mockUserService, mockRouter, mockUsers;

  const users = [
    { userId: 1, username: 'wdavidsen', firstName: 'Bill', lastName: 'Davidsen', role: 'Admin',
        lastLogin: new Date(), failedLoginCount: 0, enabled: true, selected: false },
    { userId: 2, username: 'jdoe', firstName: 'John', lastName: 'Doe', role: 'Reader',
        lastLogin: new Date(), failedLoginCount: 0, enabled: true, selected: false },
    { userId: 3, username: 'jsmith', firstName: 'Jane', lastName: 'Smith', role: 'Contributer',
        lastLogin: new Date(), failedLoginCount: 0, enabled: true, selected: false }
  ];

  class RouterStub {
    constructor() {
        this.events = new Observable<Event>();
    }
    events: Observable<Event>;
  }

  beforeEach(async(() => {
    mockUsers = [{...users[0]}, {...users[1]}, {...users[2]}];

    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockAuthenticationService = jasmine.createSpyObj(['currentUserValue', 'login', 'getCurrentUser']);
    mockUserService = jasmine.createSpyObj(['getAll', 'delete', 'save']);
    mockRouter = new RouterStub();
    mockRouter.navigate = jasmine.createSpy();

    mockAuthenticationService.getCurrentUser.and.returnValue(of(mockUsers[0]));
    mockUserService.getAll.and.returnValue(of(mockUsers));

    TestBed.configureTestingModule({
      declarations: [ UsersComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule],
      providers: [
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: Router, useValue: mockRouter },
        { provide: UserService, useValue: mockUserService},
        { provide: ToastrService, useValue: mockToastr }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.users).toBeTruthy();
    expect(component.users.length).toBe(3);
  });

  it('should select a user', () => {
    component.select(mockUsers[1]);
    expect(mockUsers[1].selected).toBe(true);

    component.select(mockUsers[1]);
    expect(mockUsers[1].selected).toBe(false);
  });

  it('should navigate to detail to add', () => {
    component.addNewUser();

    expect(mockRouter.navigate).toHaveBeenCalledTimes(1);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/users', 0]);
  });

   it('should navigate to detail to edit', () => {
    const user = mockUsers[0];
    const userId = user.userId;

    component.editUser(user);

    expect(mockRouter.navigate).toHaveBeenCalledTimes(1);
    expect(mockRouter.navigate).toHaveBeenCalledWith(['/users', userId]);
  });

  it('should delete users', () => {
    const userToDelete = component.users[1];
    userToDelete.selected = true;
    fixture.detectChanges();

    window.confirm = jasmine.createSpy('confirm').and.returnValue(true);
    mockUserService.delete.and.returnValue(of(null));

    component.deleteUser();

    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete ALL selected users?');
    expect(mockUserService.delete).toHaveBeenCalledTimes(1);
    expect(mockUserService.delete).toHaveBeenCalledWith(userToDelete.userId);
    expect(component.users.length).toBe(2);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should support cancel on delete', () => {
    const userToDelete = component.users[1];
    userToDelete.selected = true;
    fixture.detectChanges();

    window.confirm = jasmine.createSpy('confirm').and.returnValue(false);
    mockUserService.delete.and.returnValue(of(null));

    component.deleteUser();

    expect(window.confirm).toHaveBeenCalledWith('Are you sure you want to delete ALL selected users?');
    expect(mockUserService.delete).toHaveBeenCalledTimes(0);
    expect(component.users.length).toBe(3);
  });

  it('should handle enable/disable users error', () => {
    const userToUpdate = component.users[1];
    userToUpdate.selected = true;
    fixture.detectChanges();

    const sub = new Subject<any>();
    sub.error('some error');

    mockUserService.save.and.returnValue(sub.asObservable());

    component.enableDisableUser(false);

    expect(component.users[1].enabled).toBe(true);
    expect(mockToastr.error).toHaveBeenCalledTimes(1);
  });

  it('should handle delete error', () => {
    const userToDelete = component.users[1];
    userToDelete.selected = true;
    const sub = new Subject<any>();
    fixture.detectChanges();

    window.confirm = jasmine.createSpy('confirm').and.returnValue(true);
    mockUserService.delete.and.returnValue(sub.asObservable());
    sub.error('some error');

    component.deleteUser();

    expect(mockToastr.error).toHaveBeenCalledTimes(1);
    expect(component.users.length).toBe(3);
  });

  it('should enable/disable users', () => {
    const updatedUser = {...component.users[1]};
    updatedUser.enabled = false;
    component.users[1].selected = true;
    fixture.detectChanges();

    mockUserService.save.and.returnValue(of(updatedUser));

    component.enableDisableUser(false);

    expect(component.users[1].enabled).toBe(false);
    expect(mockUserService.save).toHaveBeenCalledTimes(1);
    expect(mockToastr.success).toHaveBeenCalledTimes(1);
  });

  it('should get selected users', () => {
    component.users[0].selected = true;
    component.users[1].selected = true;
    fixture.detectChanges();

    const selections = component.getSelectedUsers();

    expect(selections.length).toBe(2);
  });
});
