import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { AuthenticationService } from '../services';
import { ActivatedRoute, Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(
        private authenticationService: AuthenticationService,
        private router: Router,
        private route: ActivatedRoute) {}

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(
            catchError(err => {
                if (err instanceof HttpErrorResponse) {
                    const errorResponse = <HttpErrorResponse>err;

                    switch (errorResponse.status) {
                        case 401:
                            // does not work in chromium linux
                            // let header = errorResponse.headers.get('www-authenticate');
                            // let tokenExpired = header && header.toLocaleLowerCase().indexOf('the token expired') != -1;

                            // if (tokenExpired) {

                            const isRefresh = errorResponse.url.indexOf('/auth/refresh') !== -1;
                            const isLogin = errorResponse.url.indexOf('/auth/login') !== -1;
                            const currentUser = this.authenticationService.currentUserValue;

                            if (!isRefresh && currentUser) {
                                this.authenticationService.updateToken();
                                // this.router.navigateByUrl(err.url);
                            }
                            else if (!isLogin){
                                this.authenticationService.logout();
                                location.reload(true);
                            }
                            break;
                    }
                }

            const error = (err.error && err.error.message) ? err.error.message : err.statusText;
            return throwError(error);
        }));
    }
}
