import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { User, AccountInfo, Tokens, PasswordChange } from '../models';
import { Observable, of } from 'rxjs';
import { catchError, mapTo, tap } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AccountService {
    constructor(private http: HttpClient, private authService: AuthService) { }

    register(user: User) {
        return this.http.post(`${environment.apiUrl}/account/register`, user);
    }

    changePassword(changeInfo: PasswordChange): Observable<any> {

        return this.http.post<Tokens>(`${environment.apiUrl}/account/changePassword`, changeInfo)
            .pipe(tap(tokens => this.authService.storeTokens(tokens)));
    }

    info(user: AccountInfo): Observable<AccountInfo> {
        return this.http.get<AccountInfo>(`${environment.apiUrl}/account`);
    }

    save(user: AccountInfo): Observable<AccountInfo> {
        return this.http.put<AccountInfo>(`${environment.apiUrl}/account`, user);
    }
}
