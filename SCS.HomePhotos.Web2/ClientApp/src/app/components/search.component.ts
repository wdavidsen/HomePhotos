import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Subscription } from 'rxjs';
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
    dialogSubscription: Subscription;

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
        this.clearDialogSubscription();
    }

    sendKeywords() {
        this.searchInfo.keywords = this.keywords; 
        this.searchService.setSearchInfo(this.searchInfo);
    }

    openAdvancedSearch() {
        const options = {     
            initialState: {  
                searchInfo: this.searchInfo,
                okClicked: false,
                clearClicked: false
            }
        };
        this.advancedModalRef = this.modalService.show(SearchAdvancedComponent, options);

        this.clearDialogSubscription();
            this.dialogSubscription = this.modalService.onHidden
                .subscribe(() => {
                    const content = this.advancedModalRef.content;

                    if (content.okClicked || content.clearClicked) {
                        this.searchInfo = content.searchInfo;
                        this.keywords = this.searchInfo.keywords;
                        this.searchService.setSearchInfo(this.searchInfo);
                    }
                });
    }

    private clearDialogSubscription(): void {
        if (this.dialogSubscription) {
          this.dialogSubscription.unsubscribe();
        }
      }
}
