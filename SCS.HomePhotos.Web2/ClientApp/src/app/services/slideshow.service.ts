import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { SwiperOptions } from 'swiper';
import { PhotoSlide } from '../models';

@Injectable({ providedIn: 'root' })
export class SlideshowService {
    private configSubject = new Subject<SwiperOptions>();
    private startSubject = new Subject<Array<PhotoSlide>>();
    private stopSubject = new Subject();

    constructor() {    
    }

    configWatcher(): Observable<SwiperOptions> {
        return this.configSubject.asObservable();
    }

    startWatcher(): Observable<Array<PhotoSlide>> {
        return this.startSubject.asObservable();
    }

    stopWatcher() {
        return this.stopSubject.asObservable();
    }

    start(photos: Array<PhotoSlide>) {        
        this.startSubject.next(photos);
    }

    stop() {        
        this.stopSubject.next({});
    }

    config(options: SwiperOptions) {        
        this.configSubject.next(options);
    }
}
