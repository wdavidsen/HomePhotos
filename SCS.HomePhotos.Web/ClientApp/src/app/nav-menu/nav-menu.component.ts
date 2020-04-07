import { Component, OnInit } from '@angular/core';
import { User } from '../models';
import { AuthenticationService, SearchService } from '../services';
import { Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-nav-menu',
  templateUrl: './nav-menu.component.html',
  styleUrls: ['./nav-menu.component.css']
})
export class NavMenuComponent implements OnInit {
  isExpanded = false;
  currentUser: User;
  hideSearch = true;
  hideMenu = true;
  hideOrganize = true;

  constructor (private router: Router,
    private authenticationService: AuthenticationService,
    private searchService: SearchService) {
      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.hideMenu = !this.currentUser;
      });

      router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(info => this.SetOrganize(<NavigationEnd>info));
  }

  ngOnInit() {
    this.searchService.getHidden().subscribe(hidden => this.hideSearch = (hidden !== false));
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
  }

  private SetOrganize(navInfo: NavigationEnd): void {
    this.hideOrganize = !/\/$|\/photos|\/tags/.test(navInfo.url);
  }
}
