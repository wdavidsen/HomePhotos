import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { User } from '../models';
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
}
