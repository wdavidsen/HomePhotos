import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorageService {
  constructor() {}

  set(key: string, data: any): void {
    try {
      localStorage.setItem(key, JSON.stringify(data));
    }
    catch (e) {
      console.error('Error saving to local storage', e);
    }
  }

  get(key: string) {
    try {
      return JSON.parse(localStorage?.getItem(key) ?? '');
    }
    catch (e) {
      console.error('Error getting data from local storage', e);
      return null;
    }
  }
}
