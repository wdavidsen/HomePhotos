import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { LocalStorageService } from './local-storage.service';
import { UserSettings } from '../models/user-settings';
import { UserService } from './user.service';

@Injectable({ providedIn: 'root' })
export class UserSettingsService {
  private sub = new Subject<UserSettings>();
  private key = 'UserSettings';
  private _userSettings: UserSettings;

  get userSettings(): UserSettings { return this._userSettings; }

  constructor(private localStorge: LocalStorageService) {
    this._userSettings = this.localStorge.get(this.key) || new UserSettings();
    this.applySettings(this._userSettings);
  }

  saveSettings(settings: UserSettings) {
    this._userSettings = settings;
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
