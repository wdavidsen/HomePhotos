import { Component, OnInit, OnDestroy } from '@angular/core';
import { TagService, SearchService, OrganizeService, AuthService } from '../services';
import { TagChip, Tag, User } from '../models';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { InputDialogComponent, ConfirmDialogComponent } from '../common-dialog';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit, OnDestroy {
  tagChips: TagChip[] = [];
  organizeMode = false;

  inputModalRef: BsModalRef;
  confirmModalRef:  BsModalRef;

  private searchSubscription: Subscription;
  private organizeSubscription: Subscription;
  private dialogSubscription: Subscription;
  private currentUser: User;

  constructor(private tagService: TagService,
    private router: Router,
    private searchService: SearchService,
    private organizeService: OrganizeService,
    private toastr: ToastrService,
    private modalService: BsModalService,
    private authenticationService: AuthService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
      });
    }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.tagService.getTags()
      // .pipe(tap(tags => tags.length ? this.toastr.success(`Loaded ${tags.length} tags`) : this.toastr.warning(`No tags available`)))
      .pipe(map(tags => this.tagsToChips(tags)))
      .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));

    this.searchSubscription = this.searchService.getKeywords()
      .subscribe(keywords => {
        if (this.currentUser) {
          this.loadTags(keywords);
        }
      });

    this.organizeSubscription = this.organizeService.getEnabled()
      .subscribe(enabled => {
        this.organizeMode = enabled;
        this.clearSelections();
      });
  }

  ngOnDestroy() {
    this.searchSubscription.unsubscribe();
    this.organizeSubscription.unsubscribe();
    this.clearDialogSubscription();
  }

  select(chip: TagChip) {
    if (this.organizeMode) {
      chip.selected = !chip.selected;
    }
    else {
      this.router.navigate(['/photos', chip.name]);
    }
  }

  getSelectedChips(): TagChip[] {
    return this.tagChips.filter(chip => chip.selected);
  }

  clearSelections(): void {
    this.tagChips.forEach(thumb => thumb.selected = false);
  }

  matchesFilter(tagName: string): boolean {
    return true;
  }

  addTag() {
    const message = 'Please new tag name.';
    const options = InputDialogComponent.GetOptions('tag-add-dialog', 'New Tag Name', 'Name', message, '');
    this.inputModalRef = this.modalService.show(InputDialogComponent, options);

    this.clearDialogSubscription();
    this.dialogSubscription = this.modalService.onHidden
      .subscribe(() => {
        if (this.inputModalRef.content.okClicked) {
          this.addTagSubmit(this.inputModalRef.content.input);
        }
      });
  }

  private loadTags(keywords: string) {
    if (keywords) {
      console.log(`Received search keywords: ${keywords}`);
      this.tagService.searchTags(keywords)
        .pipe(map(tags => this.tagsToChips(tags)))
        .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));
    }
    else {
      this.tagService.getTags()
        .pipe(map(tags => this.tagsToChips(tags)))
        .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));
    }
  }

  private addTagSubmit(tagName: string) {
    if (tagName) {
      const tag: Tag = {
        tagId: 0,
        tagName: tagName
      };

      this.tagService.addTag(tag)
        .subscribe(t => {
          this.tagChips.push(this.tagToChip(t));
          this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
          this.toastr.success('Added tag successfully');
          this.clearSelections();
        },
        err => this.toastr.error('Failed to add tag')
      );
    }
  }

  renameTag() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const message = 'Please enter the new tag name.';
      const options = InputDialogComponent.GetOptions('tag-rename-dialog', 'New Tag Name', 'Name', message, chips[0].name);
      this.inputModalRef = this.modalService.show(InputDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.inputModalRef.content.okClicked) {
            this.renameTagSubmit(chips[0].id, this.inputModalRef.content.input);
          }
        });
    }
  }

  private renameTagSubmit(tagId: number, newTagName: string) {
    if (newTagName && newTagName.length) {
      const tag: Tag = {
        tagId: tagId,
        tagName: newTagName
      };

      this.tagService.updateTag(tag)
        .subscribe(t => {
          const chip = this.tagChips.find(c => c.id === t.tagId);

          if (chip) {
            chip.name = t.tagName;
          }
          this.toastr.success('Renamed tag successfully');
          this.clearSelections();
        },
        err => this.toastr.error('Failed to rename tag')
      );
    }
  }

  deleteTags() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const message = 'Are you sure you want to delete the selected tags?';
      const options = ConfirmDialogComponent.GetOptions('confirm-delete-dialog', 'Delete Tags', message, true);
      this.confirmModalRef = this.modalService.show(ConfirmDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogSubscription = this.modalService.onHidden
          .subscribe(() => {
            chips.forEach(chip => {
              if (this.confirmModalRef.content.yesClicked) {
                this.deleteTagsSubmit(chip);
              }
            });
          });
    }
  }

  private deleteTagsSubmit(chip: TagChip) {

    if (chip && chip.id) {
      this.tagService.deleteTag(chip.id)
        .subscribe(() => {
            this.toastr.success(`Deleted ${chip.name} successfully`);
            this.tagChips.splice(this.tagChips.indexOf(chip), 1);
          },
          err => this.toastr.error(`Failed to deleted ${chip.name}`)
        );
    }
  }

  copyTag() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const message = 'Please enter a name for copied tag.';
      const options = InputDialogComponent.GetOptions('tag-copy-dialog', 'Copied Tag Name', 'Name', message, chips[0].name);
      this.inputModalRef = this.modalService.show(InputDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.inputModalRef.content.okClicked) {
            this.copyTagSubmit(chips[0].id, this.inputModalRef.content.input);
          }
        });
    }
  }

  private copyTagSubmit(sourceTagId: number, copyTagName: string) {

    if (sourceTagId > 0 && copyTagName && copyTagName.length) {

      this.tagService.copyTag(sourceTagId, copyTagName)
        .subscribe(newTag => {
          const chip = this.tagToChip(newTag);
          this.tagChips.push(chip);
          this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
          this.toastr.success('Copied tag successfully');
          this.clearSelections();
        },
        err => this.toastr.error('Failed to copy tag')
      );
    }
  }

  combineTags() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const message = 'Please enter a name for the combined tags.';
      const options = InputDialogComponent.GetOptions('tag-combine-dialog', 'Combined Tag Name', 'Name', message, chips[0].name);
      this.inputModalRef = this.modalService.show(InputDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.inputModalRef.content.okClicked) {
            this.combineTagsSubmit(chips.map(c => c.id), this.inputModalRef.content.input);
          }
        });
    }
  }

  private combineTagsSubmit(sourceTagIds: number[], combinedTagName: string) {

    if (sourceTagIds && sourceTagIds.length && combinedTagName && combinedTagName.length) {

      this.tagService.mergeTags(sourceTagIds, combinedTagName)
        .subscribe(newTag => {
          sourceTagIds.forEach(id => this.tagChips.splice(this.tagChips.findIndex(c => c.id === id), 1));
          const chip = this.tagToChip(newTag);
          this.tagChips.push(chip);
          this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
          this.toastr.success('Combined tags successfully');
          this.clearSelections();
        },
        err => this.toastr.error('Failed to combine tags')
      );
    }
  }

  private clearDialogSubscription(): void {
    if (this.dialogSubscription) {
      this.dialogSubscription.unsubscribe();
    }
  }

  private tagsToChips(photos: Tag[]): TagChip[] {
    const chips = new Array<TagChip>();
    photos.forEach(tag => chips.push(this.tagToChip(tag)));
    return chips;
  }

  private tagToChip(tag: Tag): TagChip {
    const chip = new TagChip();
    chip.id = tag.tagId;
    chip.name = tag.tagName;
    chip.count = tag.photoCount > 0 ? tag.photoCount : 0;
    return chip;
  }

  private insertIndexDividers(tagChips: TagChip[]): TagChip[] {
    if (tagChips.length < 30) {
        return tagChips;
    }
    const idexes = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

    let letter = '';
    let prevLetter = '';

    const dividedChips: TagChip[] = [];

    tagChips.forEach((chip, index) => {
      letter = chip.name.substring(0, 1).toUpperCase();
      dividedChips.push(chip);

      if (letter !== prevLetter && idexes.indexOf(letter) >= 0) {
        dividedChips.push({name: letter, id: -1, isDivider: true, count: 0, selected: false});
      }
      prevLetter = letter;
    });

    return dividedChips;
  }
}
