import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { Settings } from '../models/settings';
import { Observable } from 'rxjs';

@Injectable()
export class SettingsService {
  
    constructor(private http: HttpClient) {

    }

    getSettings() : Observable<Settings> {
        return this.http.get<Settings>(`${environment.apiUrl}/settings`);
    }

    updateSettings(settings: Settings) {
        return this.http.put<Settings>(`${environment.apiUrl}/settings`, settings);
    }
}