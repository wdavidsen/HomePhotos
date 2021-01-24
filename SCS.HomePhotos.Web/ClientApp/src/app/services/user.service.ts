import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { PasswordChange, User } from '../models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    resetPassword(userId: number, changeInfo: PasswordChange): Observable<any> {
        return this.http.post(`${environment.apiUrl}/users/${userId}/resetPassword`, changeInfo);
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
