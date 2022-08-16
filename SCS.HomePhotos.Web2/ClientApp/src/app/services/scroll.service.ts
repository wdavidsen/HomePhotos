import { Injectable } from '@angular/core';
import { Observable, Subject, timer } from 'rxjs';
import { debounce } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ScrollService {
    private subject = new Subject<number>();
    private lastScroll: number;

    constructor() {
        // document.addEventListener('scroll', this.scroll, true);
    }

    scrollToBottom(): Observable<any> {
        return this.subject.asObservable().pipe(debounce(() => timer(1000)));
    }

    private scroll(event): void {
        const docElem = document.documentElement;
        let newScroll = docElem.scrollTop;

        if (newScroll < 1) {
            newScroll = document.getElementsByName('body')[0].scrollTop;
        }

        if (newScroll > this.lastScroll) {
            if ((newScroll + window.innerHeight) >= (docElem.scrollHeight - 20)) {
                this.notify(newScroll);
            }
        }
        this.lastScroll = newScroll;
    }

    private notify(position: number): void {
        this.subject.next(position);
    }
}
