import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { LocalStorageService } from './local-storage.service';
import { UserSettings } from '../models/user-settings';

@Injectable({ providedIn: 'root' })
export class UserSettingsService {
    private sub = new Subject<UserSettings>();

    constructor(localStorge: LocalStorageService) {
    }

    saveSettings(settings: UserSettings) {

        this.applySettings(settings);
    }

    getSettings(): Observable<UserSettings> {
        return this.sub.asObservable();
    }

    private applySettings(settings: UserSettings) {
        this.sub.next(settings);
    }
}
