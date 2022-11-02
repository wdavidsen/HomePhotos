import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SwiperOptions } from 'swiper';
import { SlideshowService } from '../services/slideshow.service';

import SwiperCore, { Zoom, Navigation, Pagination } from "swiper";

SwiperCore.use([Zoom, Navigation, Pagination]);

@Component({ selector: 'app-slideshow', templateUrl: 'slideshow.component.html' })
export class SlideshowComponent implements OnInit, OnDestroy {
    private subscription: Subscription;

    config: SwiperOptions = {
        slidesPerView: 3,
        spaceBetween: 50,
        navigation: true,
        pagination: { clickable: true },
        scrollbar: { draggable: true },
    };
      
    constructor(private slideshowService: SlideshowService) { }

    ngOnInit() {
        this.subscription = this.slideshowService.getCommand()
            .subscribe(data => {
                
            });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    onSwiper([swiper]) {
        console.log(swiper);
    }

    onSlideChange() {
        console.log('slide change');
      }
}
