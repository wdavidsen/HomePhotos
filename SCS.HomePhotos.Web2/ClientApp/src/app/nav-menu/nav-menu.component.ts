import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { User } from '../models';
import { AuthService, SearchService } from '../services';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { UserSettingsComponent } from '../user-settings/user-settings.component';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  @ViewChild('navbarToggler', {static: true})
  navbarToggler: ElementRef;
  @ViewChild('navbarMenu', {static: true})
  navbarMenu: ElementRef;
  userSettingsModalRef: BsModalRef;
  isExpanded = false;
  currentUser: User;
  hideSearch = true;
  hideMenu = true;
  hideMenuAdmin = true;
  hideOrganize = true;
  hideUploadMenu = true;

  constructor (private router: Router,
    private authenticationService: AuthService,
    private searchService: SearchService,
    private modalService: BsModalService,
    private toastr: ToastrService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
        this.hideMenu = !this.currentUser;

        const isAdmin = this.currentUser && this.currentUser.role === 'Admin';
        const isContributer = this.currentUser && this.currentUser.role === 'Contributor';

        this.hideMenuAdmin = !isAdmin;
        this.hideUploadMenu = !(isAdmin || isContributer);
        this.hideOrganize = !(isAdmin || isContributer);
      });

    router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(info => this.onNavigation(<NavigationEnd>info));
  }

  ngOnInit() {
    this.searchService.getHidden()
      .subscribe(hidden => {
        this.hideSearch = (hidden !== false);
      });

    this.searchService.getSearchInfo()
      .subscribe(() => {
        this.collapseNav();
      });
  }

  collapse() {
    this.isExpanded = false;
  }

  toggle() {
    this.isExpanded = !this.isExpanded;
  }

  logout() {
    this.authenticationService.logout()
      .subscribe({
        complete: () => {
          this.toastr.success('You have been signed-out');
          this.router.navigate(['/login']);
        }
      });

    this.collapseNav();
  }

  showAccountInfo() {
    this.collapseNav();

    this.router.navigate(['/account']);
  }

  showSettings() {
    this.collapseNav();

    const initialState = {
      title: 'Preferences'
    };

    this.userSettingsModalRef = this.modalService.show(UserSettingsComponent, {initialState});
  }

  navBarTogglerIsVisible() {
    return this.navbarToggler.nativeElement.offsetParent !== null;
  }

  navBarCollapsed() {
    return (<string>this.navbarMenu.nativeElement.className).indexOf('show') < 0;
  }

  collapseNav() {
    if (this.navBarTogglerIsVisible() && !this.navBarCollapsed()) {
      this.navbarToggler.nativeElement.click();
    }
  }

  private onNavigation(navInfo: NavigationEnd): void {
    this.collapseNav();

    const isAdmin = this.currentUser && this.currentUser.role === 'Admin';
    const isContributer = this.currentUser && this.currentUser.role === 'Contributor';

    this.hideOrganize = !(isAdmin || isContributer) || !/\/$|\/photos|\/tags/.test(navInfo.url);
  }
}
