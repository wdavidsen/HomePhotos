import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { User } from '../models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    changePassword(userName: string, currectPassword: string, newPassword: string, newPasswordCompare: string): Observable<any> {
        const data = {
            userName: userName,
            currectPassword: currectPassword,
            newPassword: newPassword,
            newPasswordCompare: newPasswordCompare
        };
        return this.http.post(`${environment.apiUrl}/users/changePassword`, data);
    }

    get(userId: number): Observable<User> {
        return this.http.get<User>(`${environment.apiUrl}/users/${userId}`);
    }

    getAll(): Observable<User[]> {
        return this.http.get<User[]>(`${environment.apiUrl}/users`);
    }

    delete(userId: number) {
        return this.http.delete(`${environment.apiUrl}/users/${userId}`);
    }

    save(user: User): Observable<User> {
        if (user.userId) {
            return this.http.put<User>(`${environment.apiUrl}/users/${user.userId}`, user);
        }
        else {
            return this.http.post<User>(`${environment.apiUrl}/users`, user);
        }
    }
}
