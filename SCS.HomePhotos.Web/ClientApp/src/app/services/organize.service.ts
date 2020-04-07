import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class OrganizeService {
    private subject = new Subject<any>();

    constructor(private router: Router) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.clear();
            }
        });
    }

    getEnabled(): Observable<any> {
        return this.subject.asObservable();
    }

    setEnabled(enabled: boolean) {
        this.subject.next(enabled);
    }

    clear() {
        this.subject.next(false);
    }
}
