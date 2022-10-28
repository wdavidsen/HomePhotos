import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SearchService {
    private keywordSub = new Subject<any>();
    private hiddenSub = new Subject<any>();

    constructor(private router: Router) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.clear();
            }
        });
    }

    getKeywords(): Observable<any> {
        return this.keywordSub.asObservable();
    }

    setKeywords(keywords: string) {
        this.keywordSub.next(keywords);
    }

    getHidden(): Observable<any> {
        return this.hiddenSub.asObservable();
    }

    setHidden(hidden: boolean) {
        this.hiddenSub.next(hidden);
    }

    clear() {
        this.keywordSub.next('');
        this.hiddenSub.next(true);
    }
}
