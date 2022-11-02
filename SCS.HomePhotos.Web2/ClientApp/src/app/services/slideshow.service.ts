import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SlideshowService {
    private subject = new Subject<any>();
    
    constructor() {    
    }

    getCommand(): Observable<any> {
        return this.subject.asObservable();
    }

    start(data: string) {        
        this.subject.next(data);
    }
}
