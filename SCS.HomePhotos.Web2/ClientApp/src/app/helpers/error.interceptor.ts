import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

import { AuthenticationService } from '../services';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {  
    constructor(private authenticationService: AuthenticationService) {}

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

                            let isRefresh = errorResponse.url.indexOf('/auth/refresh') != -1;
                            let isLogin = errorResponse.url.indexOf('/auth/login') != -1;
                            let currentUser = this.authenticationService.currentUserValue;

                            if (!isRefresh && currentUser) {
                                this.authenticationService.updateToken();
                                return;
                            }
                            else if (!isLogin){
                                this.authenticationService.logout();
                                location.reload(true);
                            }
                            break;
                    }
                }
            
            const error = err.error.message || err.statusText;
            return throwError(error);
        }));
    }
}
