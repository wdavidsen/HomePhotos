import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { SearchInfo } from '../models';
import { SearchService } from '../services';
import { SearchAdvancedComponent } from './search-advanced.component';

@Component({
    selector: 'app-search',
    templateUrl: 'search.component.html'
})
export class SearchComponent implements OnInit, OnDestroy {    
    keywords = ''
    searchInfo: SearchInfo = {};
    advancedModalRef: BsModalRef;  
    
    constructor(
        private searchService: SearchService,
        private modalService: BsModalService) { }

    ngOnInit() {
        this.searchService.getSearchInfo()
            .subscribe(searchInfo => {
                this.searchInfo = searchInfo;
                this.keywords = searchInfo.keywords;
            });
    }

    ngOnDestroy() {
    }

    sendKeywords() {
        this.searchInfo.keywords = this.keywords; 
        this.searchService.setSearchInfo(this.searchInfo);
    }

    openAdvancedSearch() {
        const options = {     
            initialState: {  
                searchInfo: this.searchInfo
            }
        };
        this.advancedModalRef = this.modalService.show(SearchAdvancedComponent, options);
    }
}
