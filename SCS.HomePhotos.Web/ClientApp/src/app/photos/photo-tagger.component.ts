import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TagService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { TagState } from '../models';
import { TriCheckState } from '../components/tri-check.component';

@Component({
    selector: 'app-photo-tagger',
    templateUrl: './photo-tagger.component.html',
    styleUrls: ['./photo-tagger.component.css']
  })

export class PhotoTaggerComponent implements OnInit {
    title: string;
    photoIds: number[];
    allTags: string[];
    selectedTag: string;
    tagStates: TagState[];
    dirtyTags: string[] = [];

    constructor(
        public bsModalRef: BsModalRef,
        private tagService: TagService,
        private toastr: ToastrService) {}

    ngOnInit() {
        this.loadPhotoTagStates();
        this.loadAllTags();
    }

    saveTags() {
        const updates = this.tagStates
            .filter(ts => !ts.indeterminate && this.dirtyTags.find(dt => dt === ts.tagName));

        this.tagService.updatePhotoTags(this.photoIds, updates)
            .subscribe(
                () => {
                    this.toastr.success('Successfully updated tags');
                    this.dirtyTags = [];
                    this.bsModalRef.hide();
                },
                error => {
                    console.error(error);
                    this.toastr.error('Failed to update tags');
                }
            );
    }

    addTag() {
        if (this.tagStates.find((t) => t.tagName.toUpperCase() === this.selectedTag.toUpperCase())) {
            this.toastr.warning(`Tag already applied`);
        }
        else {
            this.tagStates.push({
                tagId: 0,
                tagName: this.selectedTag,
                checked: true,
                indeterminate: false,
                allowIndeterminate: false
            });
            this.dirtyTags.push(this.selectedTag);
            this.selectedTag = null;
        }
    }

    tagStateChanged(event) {
        const triState = <TriCheckState>event;
        const tagState = this.tagStates.find(t => t.tagName === triState.label);

        tagState.checked = triState.checked;
        tagState.indeterminate = tagState.allowIndeterminate && triState.indeterminate;
        this.dirtyTags.push(tagState.tagName);
    }

    private loadPhotoTagStates() {
        this.tagService.getPhototags(this.photoIds)
            .subscribe(
                tagStates => this.tagStates = tagStates,
                error => console.error(error)
            );
    }

    private loadAllTags() {
        this.tagService.getTags()
            .subscribe(
                tags => this.allTags = tags.map(t => t.tagName),
                error => console.error(error)
            );
    }
}
