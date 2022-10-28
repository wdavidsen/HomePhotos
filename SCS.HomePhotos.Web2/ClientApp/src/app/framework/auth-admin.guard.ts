import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

import { AuthService } from '../services';

@Injectable({ providedIn: 'root' })
export class AuthAdminGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService
    ) {}

    canActivate() {
      return this.canLoad();
    }

    canLoad() {
      if (this.authService.currentUserValue.role === 'Admin') {
        return true;
      }

      this.router.navigate(['/unauthorized']);
      return false;
    }
}
