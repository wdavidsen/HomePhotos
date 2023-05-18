import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

import { User, AccountInfo, Tokens, PasswordChange, AvatarUpdate, UserSettings } from '../models';
import { Observable } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class AccountService {
    constructor(private http: HttpClient, private authService: AuthService) { }

    register(user: User) {
        return this.http.post(`${environment.apiUrl}/account/register`, user);
            // .pipe(this.handleServiceUnavailable());
    }

    changePassword(changeInfo: PasswordChange): Observable<any> {

        return this.http.post<Tokens>(`${environment.apiUrl}/account/changePassword`, changeInfo)
            .pipe(tap(tokens => this.authService.storeTokens(tokens)));
    }

    updateAvatar(file: File): Observable<AvatarUpdate> {
        const formData = new FormData();
        formData.append('image', file, file.name);

        return this.http.put<AvatarUpdate>(`${environment.apiUrl}/account/updateAvatar`, formData)
            .pipe(tap(avatarInfo => this.authService.updateAvatarImage(avatarInfo.avatarImage)));
    }

    info(user: AccountInfo): Observable<AccountInfo> {
        return this.http.get<AccountInfo>(`${environment.apiUrl}/account`);
    }

    save(user: AccountInfo): Observable<AccountInfo> {
        return this.http.put<AccountInfo>(`${environment.apiUrl}/account`, user);
    }

    saveSettings(userSettings: UserSettings): Observable<UserSettings> {
        return this.http.put<UserSettings>(`${environment.apiUrl}/account/settings`, userSettings);
    }

    getSettings(): Observable<UserSettings> {
        return this.http.get<UserSettings>(`${environment.apiUrl}/account/settings`);
    }

    // handleServiceUnavailable() {
    //     return catchError(response => {
    //         if (response.status === 0) {
    //             response.errror = { id: 'ServiceUnreachable', message: 'Service not reachable, or offline'};
    //         }
    //         return response;
    //     });
    // }
}

