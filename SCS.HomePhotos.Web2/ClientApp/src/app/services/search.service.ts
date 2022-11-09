import { Injectable } from '@angular/core';
import { Router, NavigationStart } from '@angular/router';
import { Observable, Subject } from 'rxjs';
import { SearchInfo } from '../models';

@Injectable({ providedIn: 'root' })
export class SearchService {
    private keywordSub = new Subject<SearchInfo>();
    private hiddenSub = new Subject<any>();

    constructor(private router: Router) {
        this.router.events.subscribe(event => {
            if (event instanceof NavigationStart) {
                this.clear();
            }
        });
    }

    getSearchInfo(): Observable<SearchInfo> {
        return this.keywordSub.asObservable();
    }

    setSearchInfo(keywords: SearchInfo) {
        this.keywordSub.next(keywords);
    }

    getHidden(): Observable<any> {
        return this.hiddenSub.asObservable();
    }

    setHidden(hidden: boolean) {
        this.hiddenSub.next(hidden);
    }

    clear() {
        this.keywordSub.next(null);
        this.hiddenSub.next(true);
    }
}
