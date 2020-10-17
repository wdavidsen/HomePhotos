import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { TagService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { TagState } from '../models';

@Component({
    selector: 'app-upload-photo-tagger',
    templateUrl: './upload-photo-tagger.component.html',
    styleUrls: ['./upload-photo-tagger.component.css']
  })

export class UploadPhotoTaggerComponent implements OnInit {
    title: string;
    allTags: string[];
    selectedTag: string;
    tagStates: TagState[];

    constructor(
        private tagService: TagService,
        private toastr: ToastrService) {}

    ngOnInit() {
        this.loadAllTags();
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
            this.selectedTag = null;
        }
    }

    removeTag(tagState: TagState) {
        const removeIndex =  this.tagStates.indexOf(tagState);
        this.tagStates.splice(removeIndex, 1);
    }

    private loadAllTags() {
        this.tagService.getTags()
            .subscribe(
                tags => this.allTags = tags.map(t => t.tagName),
                error => console.error(error)
            );
    }
}
