import { Component, OnInit, OnDestroy } from '@angular/core';
import { SearchService } from '../services';

@Component({
    selector: 'app-search',
    templateUrl: 'search.component.html'
})
export class SearchComponent implements OnInit, OnDestroy {
    keywords = '';

    constructor(private searchService: SearchService) { }

    ngOnInit() {
        this.searchService.getKeywords()
            .subscribe(keywords => this.keywords = keywords);
    }

    ngOnDestroy() {
    }

    sendKeywords() {
        this.searchService.setKeywords(this.keywords);
    }

    reset() {
        this.keywords = '';
        this.searchService.setKeywords('');
    }
}
