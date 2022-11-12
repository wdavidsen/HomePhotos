import { OnInit, Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { SearchInfo } from '../models';

@Component({
    selector: 'app-search-advanced',
    templateUrl: './search-advanced.component.html'    
  })

export class SearchAdvancedComponent implements OnInit {    
    searchInfo: SearchInfo;
    searchForm: UntypedFormGroup;
    clearClicked = false;
    okClicked = false;
    closing = false;
    
    constructor(
        private formBuilder: UntypedFormBuilder,
        public bsModalRef: BsModalRef) {}

    ngOnInit() {
        this.closing = false;
        this.setupForm(this.searchInfo);        
    }

    clear() {    
        this.closing = true;  
        this.clearClicked = true;
        this.okClicked = false;
        this.searchForm.reset();
        this.searchInfo = this.formToSearchInfo();        
        this.bsModalRef.hide();
    }

    cancel() {  
        this.closing = true;  
        this.clearClicked = false;
        this.okClicked = false;     
        this.bsModalRef.hide();
    }

    onSubmit() {
        if (this.closing) return;
        this.closing = true;  
        this.okClicked = true;
        this.clearClicked = false;
        this.searchInfo = this.formToSearchInfo();        
        this.bsModalRef.hide();
    }

    // convenience getter for easy access to form fields
  get f() { return this.searchForm.controls; }

  private setupForm(data: SearchInfo) {

    this.searchForm = this.formBuilder.group({
      fromDate: [data ? data.fromDate : null],
      toDate: [data ? data.toDate : null],
      keywords: [data ? data.keywords : null],
    });
  }

  private formToSearchInfo(): SearchInfo {
    const settings = new SearchInfo();

    settings.fromDate = this.f.fromDate.value;
    settings.toDate = this.f.toDate.value;
    settings.keywords = this.f.keywords.value;

    return settings;
  }
}