import { OnInit, Component } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { SearchInfo } from '../models';
import { SearchService } from '../services';

@Component({
    selector: 'app-search-advanced',
    templateUrl: './search-advanced.component.html'    
  })

export class SearchAdvancedComponent implements OnInit {    
    searchInfo: SearchInfo;
    searchForm: UntypedFormGroup;
    
    constructor(
        private formBuilder: UntypedFormBuilder,
        public bsModalRef: BsModalRef,
        private searchService: SearchService,        
        private toastr: ToastrService) {}

    ngOnInit() {
        this.setupForm(this.searchInfo);
    }

    clear() {      
        this.searchForm.reset();
        this.onSubmit();
    }

    cancel() {        
        this.bsModalRef.hide();
    }

    onSubmit() {
        this.searchInfo = this.formToSearchInfo();
        this.searchService.setSearchInfo(this.searchInfo);
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
