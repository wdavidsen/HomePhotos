import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable()
export class CsrfHeaderInterceptor implements HttpInterceptor {
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        if (request.method === 'POST' || request.method === 'PUT') {
            const requestToken = this.getCookieValue('XSRF-REQUEST-TOKEN');

            if (requestToken === null || requestToken === '') {
                console.error('The CSRF token is missing!');
            }

            return next.handle(request.clone({
                headers: request.headers.set('X-XSRF-TOKEN', requestToken)
            }));
        }
        else {
            return next.handle(request);
        }
    }

    private getCookieValue(cookieName: string) {
        const allCookies = decodeURIComponent(document.cookie).split('; ');

        for (let i = 0; i < allCookies.length; i++) {
            const cookie = allCookies[i];

            if (cookie.startsWith(cookieName + '=')) {
                return cookie.substring(cookieName.length + 1);
            }
        }
        return '';
    }
}
