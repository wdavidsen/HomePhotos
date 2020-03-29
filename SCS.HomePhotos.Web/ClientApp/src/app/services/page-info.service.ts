import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PageInfoService {
    private subject = new Subject<any>();

    constructor(private router: Router) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.clear();
            }
        });
    }

    getTitle(): Observable<any> {
        return this.subject.asObservable();
    }

    setTitle(title: string) {
        this.subject.next(title);
    }

    clear() {
        this.subject.next();
    }
}
