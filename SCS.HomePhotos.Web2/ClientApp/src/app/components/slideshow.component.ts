import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { SwiperOptions } from 'swiper';
import { SlideshowService } from '../services/slideshow.service';
import { EventsParams, SwiperComponent } from "swiper/angular";

import SwiperCore, { Lazy, Autoplay, Zoom, Navigation } from "swiper";
import { PhotoSlide } from '../models';

SwiperCore.use([Lazy, Autoplay, Zoom, Navigation]);

@Component({ selector: 'app-slideshow', templateUrl: 'slideshow.component.html' })
export class SlideshowComponent implements OnInit, OnDestroy {
    private configSub: Subscription;
    private startSub: Subscription;
    private stopSub: Subscription;

    @ViewChild('swiper', { static: false }) 
    swiper?: SwiperComponent;    
    config: SwiperOptions = {};      
    started: boolean = false;

    slides: Array<PhotoSlide> = [];
    pendingSlides: Array<PhotoSlide> = [];
    
    constructor(private slideshowService: SlideshowService) { }

    ngOnInit() {   
        this.slideshowService.configWatcher()
            .subscribe(config => {
                this.config = config;
                console.log('Updated slideshow configuration.');
            });

        this.startSub = this.slideshowService.startWatcher()
            .subscribe(slides => {
                this.pendingSlides = slides;
                this.started = true;                
                // this.swiper.swiperRef <- will be undefined at this point.
                console.log('Started slideshow.');
            });

        this.stopSub = this.slideshowService.stopWatcher()
            .subscribe(_ => {
                this.swiper.swiperRef.autoplay.stop();
                this.started = false;
                console.log('Stopped slideshow.');
            });
    }

    ngOnDestroy() {
        this.configSub.unsubscribe();
        this.startSub.unsubscribe();
        this.stopSub.unsubscribe();
    }

    onSwiper(params: EventsParams['afterInit']) {        
        //this.swiper.swiperRef.addSlide();
        const startIndex = this.pendingSlides
            .map(s => s.selected)
            .indexOf(true);
            
        this.slides = this.pendingSlides;

        if (startIndex > -1) {
            this.swiper.swiperRef.slideTo(startIndex);
        }
        this.swiper.swiperRef.autoplay.start();
        console.log('Initialized slideshow swiper.');
    }
}
