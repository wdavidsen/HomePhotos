import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { PageInfoService } from '../services';

@Component({
    selector: 'app-page-info',
    templateUrl: 'page-info.component.html',
    styleUrls: ['./page-info.component.css']
})
export class PageInfoComponent implements OnInit, OnDestroy {
    private subscription: Subscription;
    title: any;

    constructor(private pageInfoService: PageInfoService) { }

    ngOnInit() {
        this.subscription = this.pageInfoService.getTitle()
            .subscribe(title => {
                this.title = title;
            });
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }
}
