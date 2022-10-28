import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorageService {
  constructor() {}

  set(key: string, data: any): void {
    if (!localStorage) {
      console.error('Local storage is unavailable.');
    }

    var json = JSON.stringify(data);
    localStorage.setItem(key, json);
  }

  get(key: string) {
    if (!localStorage) {
      console.error('Local storage is unavailable.');
    }
    var json = localStorage.getItem(key);
    return json ? JSON.parse(json) : '';
  }
}
