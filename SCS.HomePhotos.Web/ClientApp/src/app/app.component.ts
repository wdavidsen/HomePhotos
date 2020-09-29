import { Component } from '@angular/core';
import { Router, NavigationEnd } from '@angular/router';
import { AuthenticationService, SignalRService } from './services';
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

  constructor (private router: Router, private authenticationService: AuthenticationService, private signalRService: SignalRService,
    private toastr: ToastrService) {
      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;

        if (user.role === 'Admin') {
          this.signalRService.startConnection();
          this.signalRService.listenForAdminMessages();
          this.signalRService.getMessages().subscribe((info) => {
            console.log(info.text);

            let toastFunc = toastr.info;
            switch ((info.type || '').toLowwerCase()) {
              case 'error':
                toastFunc = toastr.error;
                break;
              case 'warn':
                toastFunc = toastr.warning;
                break;
            }

            toastFunc(info.message, info.title || '', { tapToDismiss: true, disableTimeOut: true });
          });
        }
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
