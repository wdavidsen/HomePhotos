import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

import { User } from '../models';

@Injectable({ providedIn: 'root' })
export class AuthenticationService {
    private currentUserSubject: BehaviorSubject<User>;
    public currentUser: Observable<User>;

    constructor(private http: HttpClient) {
        this.currentUserSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('currentUser')));
        this.currentUser = this.currentUserSubject.asObservable();
    }

    public get currentUserValue(): User {
        return this.currentUserSubject.value;
    }

    login(username, password) {
        return this.http.post<any>(`${environment.apiUrl}/auth/login`, { username, password })
            .pipe(map(user => {
                // store user details and jwt token in local storage to keep user logged in between page refreshes
                localStorage.setItem('currentUser', JSON.stringify(user));
                this.currentUserSubject.next(user);
                return user;
            }));
    }

    logout() {
        // remove user from local storage and set current user to null
        localStorage.removeItem('currentUser');
        this.currentUserSubject.next(null);
    }

    updateToken() {
        var currentUser = this.currentUserValue;
        var payload = { 
            token: currentUser.token, 
            refreshToken: currentUser.refreshToken 
        };

        return this.http.post<any>(`${environment.apiUrl}/auth/refresh`, payload)
            .subscribe(tokenInfo => {
                var updatedUser = Object.assign({}, currentUser);
                updatedUser.token = tokenInfo.token;
                updatedUser.refreshToken = tokenInfo.refreshToken;

                localStorage.setItem('currentUser', JSON.stringify(updatedUser));
                this.currentUserSubject.next(updatedUser);                
                location.reload(true);
              },
              error => {
                  console.error(error);
                  this.logout();
                  location.reload(true);
              });
    }

    requiresAuthentication(url: string) {

        if (url.indexOf('/auth/login') != -1) {
            return false;
        }
        if (url.indexOf('/auth/register') != -1) {
            return false;
        }
        return true;
    }
}
