import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthenticationService } from './services';
import { User } from './models';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  currentUser: User;
  title = 'app';
  containerClass = 'container';

  constructor (private router: Router, private authenticationService: AuthenticationService) {
      this.authenticationService.currentUser.subscribe(x => this.currentUser = x);

      router.events
        .pipe(filter(event => event instanceof NavigationEnd))
        .subscribe(info => this.SetContainer(<NavigationEnd>info));
  }

  logout() {
    this.authenticationService.logout();
    this.router.navigate(['/login']);
  }

  private SetContainer(navInfo: NavigationEnd): void {
    this.containerClass =  (/\/$|\/photos|\/tags/.test(navInfo.url)) ? 'container-fluid' : 'container';
  }
}
