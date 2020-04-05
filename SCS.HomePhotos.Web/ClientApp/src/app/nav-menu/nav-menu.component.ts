import { Component, OnInit } from '@angular/core';
import { User } from '../models';
import { AuthenticationService, SearchService } from '../services';
import { Router } from '@angular/router';

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

  constructor (private router: Router,
    private authenticationService: AuthenticationService,
    private searchService: SearchService) {
      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
        this.hideMenu = !this.currentUser;
      });
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
}
