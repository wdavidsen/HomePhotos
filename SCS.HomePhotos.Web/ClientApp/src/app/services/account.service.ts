import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { User, AccountInfo } from '../models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AccountService {
    constructor(private http: HttpClient) { }

    register(user: User) {
        return this.http.post(`${environment.apiUrl}/account/register`, user);
    }

    changePassword(currectPassword: string, newPassword: string, newPasswordCompare: string): Observable<any> {
        const data = {
            currectPassword: currectPassword,
            newPassword: newPassword,
            newPasswordCompare: newPasswordCompare
        };
        return this.http.post(`${environment.apiUrl}/account/changePassword`, data);
    }

    info(user: AccountInfo): Observable<AccountInfo> {
        return this.http.get<AccountInfo>(`${environment.apiUrl}/account`);
    }

    save(user: AccountInfo): Observable<AccountInfo> {
        return this.http.put<AccountInfo>(`${environment.apiUrl}/account`, user);
    }
}
