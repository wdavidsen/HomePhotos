import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';

import { AuthService } from '../services';

@Injectable({ providedIn: 'root' })
export class AuthUploadGuard implements CanActivate {
    constructor(
        private router: Router,
        private authService: AuthService
    ) {}

    canActivate() {
      return this.canLoad();
    }

    canLoad() {
      const role = this.authService.currentUserValue.role;

      if (role === 'Admin' || role === 'Contributer') {
        return true;
      }

      this.router.navigate(['/unauthorized']);
      return false;
    }
}
