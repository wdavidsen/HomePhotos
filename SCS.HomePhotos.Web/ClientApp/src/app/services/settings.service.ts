import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Settings } from '../models/settings';
import { Observable } from 'rxjs';

@Injectable()
export class SettingsService {

    constructor(private http: HttpClient) {

    }

    getSettings(): Observable<Settings> {
        return this.http.get<Settings>(`${environment.apiUrl}/settings`);
    }

    updateSettings(settings: Settings, reprocessPhotos?: boolean): Observable<void> {
        let url = `${environment.apiUrl}/settings`;

        if (reprocessPhotos) {
            url += '?reprocessPhotos=true';
        }
        return this.http.put<void>(url, settings);
    }

    indexNow(reprocessPhotos?: boolean): Observable<Settings> {
        let url = `${environment.apiUrl}/settings/indexNow`;

        if (reprocessPhotos) {
            url += '?reprocessPhotos=true';
        }
        return this.http.put<Settings>(url, {});
    }

    clearCache(): Observable<void> {
        const url = `${environment.apiUrl}/settings/clearCache`;

        return this.http.put<void>(url, {});
    }
}
