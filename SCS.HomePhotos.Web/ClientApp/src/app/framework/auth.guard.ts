import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

import { AuthService } from '../services';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService
    ) {}

    canActivate() {
      return this.canLoad();
    }

    canLoad() {
      if (!this.authService.isLoggedIn()) {
        this.router.navigate(['/login']);
      }
      return this.authService.isLoggedIn();
    }
}
