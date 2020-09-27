import { Component, OnInit, ElementRef, ViewChild } from '@angular/core';
import { User } from '../models';
import { AuthenticationService, SearchService } from '../services';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { UserSettingsComponent } from '../user-settings/user-settings.component';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  @ViewChild('navbarToggler', {static: true})
  navbarToggler: ElementRef;
  userSettingsModalRef: BsModalRef;
  isExpanded = false;
  currentUser: User;
  hideSearch = true;
  hideMenu = true;
  hideMenuAdmin = true;
  hideOrganize = true;
  hideUploadMenu = true;

  constructor (private router: Router,
    private authenticationService: AuthenticationService,
    private searchService: SearchService,
    private modalService: BsModalService) {
      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.hideMenu = !this.currentUser;
        this.hideMenuAdmin = !(this.currentUser && this.currentUser.role === 'Admin');
        this.hideUploadMenu = this.hideMenuAdmin;
      });

      router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(info => this.SetOrganize(<NavigationEnd>info));
  }

  ngOnInit() {
    this.searchService.getHidden()
      .subscribe(hidden => {
        this.hideSearch = (hidden !== false);
        console.log(`Search hide state: ${hidden}`);
      });

    this.searchService.getKeywords()
      .subscribe(hidden => {
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
    this.authenticationService.logout();
    this.router.navigate(['/login']);
    this.collapseNav();
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

  collapseNav() {
    if (this.navBarTogglerIsVisible()) {
      this.navbarToggler.nativeElement.click();
    }
  }

  private SetOrganize(navInfo: NavigationEnd): void {
    this.hideOrganize = !/\/$|\/photos|\/tags/.test(navInfo.url);
  }
}
