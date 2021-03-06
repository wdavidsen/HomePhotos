import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthService, SignalRService } from './services';
import { User } from './models';
import { filter } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  currentUser: User;
  title = 'app';
  containerClass = 'container';

  constructor (private router: Router, private authenticationService: AuthService, private signalRService: SignalRService,
    private toastr: ToastrService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;

        this.signalRService.startConnection();
        this.signalRService.listenForEveryoneMessages();

        if (user && user.role === 'Admin') {
          this.signalRService.listenForAdminMessages();
        }

        this.signalRService.getMessages().subscribe((info) => {
          console.log(`${info.type}: ${info.message}`);
          toastr.show(info.message, '', { tapToDismiss: true, disableTimeOut: true }, info.type);
        });
      });

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
