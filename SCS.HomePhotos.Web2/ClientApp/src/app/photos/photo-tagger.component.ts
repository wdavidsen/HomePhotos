import { OnInit, Component, ViewChild, AfterViewInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { AuthService, TagService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { TagState } from '../models';
import { TriCheckState } from '../components/tri-check.component';
import { TabsetComponent } from 'ngx-bootstrap/tabs';

@Component({
    selector: 'app-photo-tagger',
    templateUrl: './photo-tagger.component.html',
    styleUrls: ['./photo-tagger.component.css']
  })

export class PhotoTaggerComponent implements OnInit, AfterViewInit  {
    title: string;
    photoIds: number[];
    allTags_shared: string[];
    selectedTag_shared: string;
    tagStates_shared: TagState[];
    dirtyTags_shared: string[] = [];    
    allTags_personal: string[];
    selectedTag_personal: string;
    tagStates_personal: TagState[];
    dirtyTags_personal: string[] = [];    
    caller: string = 'PhotoList';
    username: string;
    @ViewChild('tabset', { static: false }) tabset?: TabsetComponent;
 
    constructor(
        public bsModalRef: BsModalRef,
        private authenticationService: AuthService,
        private tagService: TagService,
        private toastr: ToastrService) {
            this.authenticationService.getCurrentUser().subscribe(user => {
                this.username = user.username;
            });
        }

    ngOnInit() {    
        if (this.caller.toUpperCase() == 'PHOTOLIST') {
            this.loadSharedPhotoTagStates();
            this.loadPersonalPhotoTagStates();
        }
        this.loadAllSharedTags();
        this.loadAllPersonalTags();  
    }

    ngAfterViewInit() {
        // console.log('tab count: ' + this.tabset.tabs.length);
        // this.tabset.tabs[1].disabled = true;
        // this.tabset.tabs[1].customClass = 'tab-disabled';
    }
      
    ok() {
        this.bsModalRef.hide();
        
        if (this.caller.toUpperCase() == 'PHOTOUPLOAD') {
            return;
        }

        const updatesShared = this.tagStates_shared
            .filter(ts => !ts.indeterminate && this.dirtyTags_shared.find(dt => dt === ts.tagName));

        if (updatesShared.length) {
            this.tagService.updatePhotoTags(this.photoIds, updatesShared)
                .subscribe({
                    next: () => {
                        this.toastr.success('Successfully updated shared tags');
                        this.dirtyTags_shared = [];                        
                    },
                    error: (e) => { console.error(e); this.toastr.error('Failed to update shared tags'); }
                });       
        }    

        const updatesPersonal = this.tagStates_personal
            .filter(ts => !ts.indeterminate && this.dirtyTags_personal.find(dt => dt === ts.tagName));

        if (updatesPersonal.length) {
            this.tagService.updatePhotoTags(this.photoIds, updatesPersonal, this.username)
                .subscribe({
                    next: () => {
                        this.toastr.success('Successfully updated personal tags');
                        this.dirtyTags_personal = [];                        
                    },
                    error: (e) => { console.error(e); this.toastr.error('Failed to update personal tags'); }
                });       
        }    
    }

    addTag_shared() {
        if (!this.selectedTag_shared.length) {
            return;
        }
        this.tagStates_shared ??= [];

        if (this.tagStates_shared.find((t) => t.tagName.toUpperCase() === this.selectedTag_shared.toUpperCase())) {
            this.toastr.warning(`Tag already applied`);
            return;
        }

        this.tagStates_shared.push({
            tagId: 0,
            tagName: this.selectedTag_shared,
            checked: true,
            indeterminate: false,
            allowIndeterminate: false
        });
        this.dirtyTags_shared.push(this.selectedTag_shared);
        this.selectedTag_shared = null;
    }

    addTag_personal() {
        if (!this.selectedTag_personal.length) {
            return;
        }
        this.tagStates_personal ??= [];

        if (this.tagStates_personal.find((t) => t.tagName.toUpperCase() === this.selectedTag_personal.toUpperCase())) {
            this.toastr.warning(`Tag already applied`);
            return;
        }

        this.tagStates_personal.push({
            tagId: 0,
            tagName: this.selectedTag_personal,
            checked: true,
            indeterminate: false,
            allowIndeterminate: false
        });
        this.dirtyTags_personal.push(this.selectedTag_personal);
        this.selectedTag_personal = null;
    }

    tagStateChanged_shared(event) {
        const triState = <TriCheckState>event;
        const tagState = this.tagStates_shared.find(t => t.tagName === triState.label);

        tagState.checked = triState.checked;
        tagState.indeterminate = tagState.allowIndeterminate && triState.indeterminate;
        this.dirtyTags_shared.push(tagState.tagName);
    }

    tagStateChanged_personal(event) {
        const triState = <TriCheckState>event;
        const tagState = this.tagStates_personal.find(t => t.tagName === triState.label);

        tagState.checked = triState.checked;
        tagState.indeterminate = tagState.allowIndeterminate && triState.indeterminate;
        this.dirtyTags_personal.push(tagState.tagName);
    }

    private loadSharedPhotoTagStates() {
        this.tagService.getPhototags(this.photoIds)
            .subscribe({
                next: (states) => this.tagStates_shared = states ?? [],
                error: (e) => console.error(e)
            });
    }

    private loadPersonalPhotoTagStates() {
        this.tagService.getPhototags(this.photoIds, this.username)
            .subscribe({
                next: (states) => this.tagStates_personal = states ?? [],
                error: (e) => console.error(e)
            });
    }

    private loadAllSharedTags() {
        this.tagService.getTags()
            .subscribe({
                next: (tags) => this.allTags_shared = tags.map(t => t.tagName) ?? [],
                error: (e) => console.error(e)
            });
    }

    private loadAllPersonalTags() {
        this.tagService.getTags(this.username)
            .subscribe({
                next: (tags) => this.allTags_personal = tags.map(t => t.tagName) ?? [],
                error: (e) => console.error(e)
            });
    }
}
