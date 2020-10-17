import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { LocalStorageService } from './local-storage.service';
import { UserSettings } from '../models/user-settings';

@Injectable({ providedIn: 'root' })
export class UserSettingsService {
  private sub = new Subject<UserSettings>();
  private key = 'UserSettings';
  private userSettings: UserSettings;

  constructor(private localStorge: LocalStorageService) {
    this.userSettings = this.localStorge.get(this.key) || new UserSettings();
    this.applySettings(this.userSettings);
  }

  saveSettings(settings: UserSettings) {
    this.userSettings = settings;
    this.localStorge.set(this.key, settings);
    this.applySettings(settings);
  }

  getSettings(): Observable<UserSettings> {
    return this.sub.asObservable();
  }

  private applySettings(settings: UserSettings) {
    this.sub.next(settings);
  }
}
