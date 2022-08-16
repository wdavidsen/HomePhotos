import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AuthService, SearchService } from '../services';

import { LocalStorageService } from '../services/local-storage.service';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { Observable, of, Subject } from 'rxjs';
import { NavigationEnd, Router } from '@angular/router';
import { NavMenuComponent } from './nav-menu.component';
import { Component, NO_ERRORS_SCHEMA } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

describe('NavMenuComponent', () => {
  let component: NavMenuComponent;
  let fixture: ComponentFixture<NavMenuComponent>;
  let mockAuthenticationService, mockSearchService, mockModalService, mockRouter, mockToastr;

  const navSubject = new Subject<NavigationEnd>();
  const showSearchSubject = new Subject<any>();

    @Component({
        selector: 'app-search',
        template: '<div></div>'
    })
    class SearchComponent {
        keywords = '';
    }

    class RouterStub {
        events: Observable<NavigationEnd> = navSubject.asObservable();
        navigate = jasmine.createSpy();
    }

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockSearchService = jasmine.createSpyObj(['getHidden', 'getKeywords']);
    mockAuthenticationService = jasmine.createSpyObj(['getCurrentUser', 'logout', 'loadCsrfToken']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);
    mockRouter = new RouterStub();

    mockSearchService.getHidden.and.returnValue(showSearchSubject.asObservable());
    mockSearchService.getKeywords.and.returnValue(of(null));
    mockAuthenticationService.logout.and.returnValue(of(null));

    TestBed.configureTestingModule({
      declarations: [ NavMenuComponent ],
      imports: [ModalModule.forRoot()],
      providers: [
        LocalStorageService,
        SearchService,
        { provide: Router, useValue: mockRouter },
        { provide: SearchService, useValue: mockSearchService },
        { provide: AuthService, useValue: mockAuthenticationService },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }],
        schemas: [NO_ERRORS_SCHEMA]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    mockAuthenticationService.getCurrentUser.and.returnValue(of({userId: 1, username: 'jsmith', role: 'Reader'}));
    navSubject.next(new NavigationEnd(0, '/photos', null));
    showSearchSubject.next(false);

    fixture = TestBed.createComponent(NavMenuComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should log-out', () => {
    component.logout();

    expect(mockRouter.navigate).toHaveBeenCalledTimes(1);
    expect(mockAuthenticationService.logout).toHaveBeenCalledTimes(1);
  });

  it('should show search', () => {
    showSearchSubject.next(true);
    component.ngOnInit();
    fixture.detectChanges();

    expect(component.hideSearch).toBe(true);
  });

  describe('Admin Context', () => {

    beforeEach(() => {
      mockAuthenticationService.getCurrentUser.and.returnValue(of({userId: 1, username: 'wdavidsen', role: 'Admin'}));
      navSubject.next(new NavigationEnd(0, '/photos', null));
      showSearchSubject.next(false);

      fixture = TestBed.createComponent(NavMenuComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('should render admin menu', () => {
      expect(component.hideMenuAdmin).toBe(false);
      expect(fixture.nativeElement.querySelector('#adminMenu').getAttribute('hidden')).toBeNull();
    });

    it('should render organize section', () => {
      expect(component.hideOrganize).toBe(false);
      expect(fixture.nativeElement.querySelector('#organizeSection').getAttribute('hidden')).toBeNull();
    });

    it('shouldshow organize section appropriately', () => {
      navSubject.next(new NavigationEnd(0, '/users', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/settings', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/login', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/register', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/account', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/logs', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/tags', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(false);

      navSubject.next(new NavigationEnd(0, '/upload', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(true);

      navSubject.next(new NavigationEnd(0, '/photos', null));
      fixture.detectChanges();
      expect(component.hideOrganize).toBe(false);
    });
  });

  describe('Non-Admin Context', () => {

    beforeEach(() => {
      mockAuthenticationService.getCurrentUser.and.returnValue(of({userId: 1, username: 'jdoe', role: 'Contributer'}));
      navSubject.next(new NavigationEnd(0, '/photos', null));
      showSearchSubject.next(false);

      fixture = TestBed.createComponent(NavMenuComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('should not render admin menu', () => {
      expect(component.hideMenuAdmin).toBe(true);
      expect(fixture.nativeElement.querySelector('#adminMenu').getAttribute('hidden')).not.toBeNull();
    });
  });

  describe('Non-Contributer Context', () => {

    beforeEach(() => {
      mockAuthenticationService.getCurrentUser.and.returnValue(of({userId: 1, username: 'jdoe', role: 'Reader'}));
      navSubject.next(new NavigationEnd(0, '/photos', null));
      showSearchSubject.next(false);

      fixture = TestBed.createComponent(NavMenuComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('should not render admin menu', () => {
      expect(component.hideMenuAdmin).toBe(true);
      expect(fixture.nativeElement.querySelector('#adminMenu').getAttribute('hidden')).not.toBeNull();
    });

    it('should not render organize section', () => {
      expect(component.hideOrganize).toBe(true);
      expect(fixture.nativeElement.querySelector('#organizeSection').getAttribute('hidden')).not.toBeNull();
    });
  });

  describe('No User Context', () => {

    beforeEach(() => {
      mockAuthenticationService.getCurrentUser.and.returnValue(of(null));
      navSubject.next(new NavigationEnd(0, '/login', null));
      showSearchSubject.next(false);

      fixture = TestBed.createComponent(NavMenuComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
    });

    it('should not show main menu', () => {
      expect(component.hideMenu).toBe(true);
      expect(fixture.nativeElement.querySelector('#mainMenu').getAttribute('hidden')).not.toBeNull();
    });

    it('should not show right menu', () => {
      expect(component.hideMenu).toBe(true);
      expect(fixture.nativeElement.querySelector('#rightMenu').getAttribute('hidden')).not.toBeNull();
    });
  });
});
