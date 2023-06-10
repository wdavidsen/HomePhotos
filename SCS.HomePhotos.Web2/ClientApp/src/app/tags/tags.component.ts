import { Component, OnInit, OnDestroy } from '@angular/core';
import { TagService, SearchService, OrganizeService, AuthService, UserSettingsService } from '../services';
import { TagChip, Tag, User, SearchInfo } from '../models';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDialogComponent } from '../common-dialog';
import { UserSettings } from '../models/user-settings';
import { TagDialogComponent } from './tag-dialog.component';
import { CopyTagDialogComponent } from './copy-tag-dialog.component';
import { HttpErrorResponse } from '@angular/common/http';

declare var RGB_Log_Shade: any;

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit, OnDestroy {
  tagChips: TagChip[] = [];
  selectedTagChips: TagChip[] = [];
  organizeMode = false;
  filterLetter: string = null;  
  inputModalRef: BsModalRef;
  confirmModalRef: BsModalRef;
  tagModalRef: BsModalRef;
  copyTagModalRef: BsModalRef;

  private searchSubscription: Subscription;
  private organizeSubscription: Subscription;
  private dialogHiddenSubscription: Subscription;
  private userSettingsSubscription: Subscription;
  private currentUser: User;
  private userSettings: UserSettings;
  
  constructor(private tagService: TagService,
    private router: Router,
    private searchService: SearchService,
    private userSettingsService: UserSettingsService,
    private organizeService: OrganizeService,
    private toastr: ToastrService,
    private modalService: BsModalService,
    private authenticationService: AuthService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
        this.userSettings = this.userSettingsService.userSettings;
      });
    }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.tagService.getTags()
      // .pipe(tap(tags => tags.length ? this.toastr.success(`Loaded ${tags.length} tags`) : this.toastr.warning(`No tags available`)))
      .pipe(map(tags => this.tagsToChips(tags)))
      .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));

    this.searchSubscription = this.searchService.getSearchInfo()
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

    this.userSettingsSubscription = this.userSettingsService.getSettings()
      .subscribe(settings => this.userSettings = settings);
  }

  ngOnDestroy() {
    this.searchSubscription.unsubscribe();
    this.organizeSubscription.unsubscribe();
    this.userSettingsSubscription.unsubscribe();
    this.clearDialogSubscription();
  }

  select(chip: TagChip) {
    if (this.organizeMode) {
      chip.selected = !chip.selected;
      if (chip.selected) {
        this.selectedTagChips.push(chip);
      }
      else {
        this.selectedTagChips = this.selectedTagChips.filter(c => !(c.id == chip.id && c.ownerUsername == chip.ownerUsername));
      }
    }
    else {
      if (chip.ownerUsername) {        
        this.router.navigate(['/photos', chip.ownerUsername, chip.name]);
      }
      else {
        this.router.navigate(['/photos', chip.name]);
      }
    }
  }

  getSelectedChips(): TagChip[] {
    return this.tagChips.filter(chip => chip.selected);
  }

  clearSelections(): void {
    this.tagChips.forEach(thumb => thumb.selected = false);
    this.selectedTagChips = [];
  }

  toggleFilter(tagName: string) {
    this.filterLetter = this.filterLetter === null ? tagName : null;
  }

  matchesFilter(tagName: string): boolean {
    return this.filterLetter === null || tagName.startsWith(this.filterLetter);
  }

  addTag() {
    const message = '';
    const showRadios = User.isAdmin(this.currentUser) || User.isContributer(this.currentUser); 
    const options = TagDialogComponent.GetOptions('tag-add-dialog', 'New Tag', 'Tag Name', message, '', 'S', showRadios, false);
    this.tagModalRef = this.modalService.show(TagDialogComponent, options);

    this.clearDialogSubscription();
    this.dialogHiddenSubscription = this.modalService.onHidden
      .subscribe(() => {
        if (this.tagModalRef.content.okClicked) {
          const c = this.tagModalRef.content;
          const tagName = c.tagName;

          if (tagName && tagName.trim()) {
            const tag: Tag = {
              tagName: c.tagName,
              ownerId: c.tagType == 'S' ? null : this.currentUser.userId
            };
            this.addTagSubmit(tag);
          }          
        }
      });
  }

  renameTag() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const chip = chips[0];
      const tagType = chip.ownerId ? 'P' : 'S';
      const message = 'Please enter the new tag name.';
      const showRadios = User.isAdmin(this.currentUser) || User.isContributer(this.currentUser);       
      const options = TagDialogComponent.GetOptions('tag-rename-dialog', 'New Tag Name', 'Tag Name', message, chip.name, tagType, showRadios, true);
      this.tagModalRef = this.modalService.show(TagDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogHiddenSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.tagModalRef.content.okClicked) {
            const c = this.tagModalRef.content;
            const tagName = c.tagName;

            if (tagName && tagName.trim() && tagName.toUpperCase() != chip.name.toUpperCase()) {
              chip.name = this.tagModalRef.content.tagName;
              const tag = TagChip.toTag(chip);
              this.renameTagSubmit(tag);
            }
          }
        });
    }
  }

  private loadTags(searchInfo: SearchInfo) {
    if (searchInfo.changingContext) {
      return;
    }
    if (searchInfo.keywords || searchInfo.fromDate || searchInfo.toDate) {
      console.log(`Received search. keywords: ${searchInfo.keywords}; from date: ${searchInfo.fromDate}; to date: ${searchInfo.toDate}`);
      this.tagService.searchTags(searchInfo)
        .pipe(map(tags => this.tagsToChips(tags)))
        .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));
    }
    else {
      this.tagService.getTags()
        .pipe(map(tags => this.tagsToChips(tags)))
        .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));
    }
  }

  private addTagSubmit(tag: Tag) {
    if (tag.tagName) {     
      this.tagService.addTag(tag)
        .subscribe({
          next: (t) => {
            t.tagColor = t.ownerId ? this.currentUser.tagColor : TagChip.defaultColor;
            const chip = this.tagToChip(t);
            chip.selected = true;
            this.tagChips.push(chip);
            this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
            this.toastr.success('Added tag successfully');
          },
          error: (response: HttpErrorResponse) => { console.error(response); this.toastr.error(response.error.message ?? 'Failed to add tag') }
        });
    }
  }

  private renameTagSubmit(tag: Tag) {   
    if (tag.tagName) { 
      this.tagService.updateTag(tag)
        .subscribe({
          next: (t) => {
            const chip = this.tagChips.find(c => c.id === t.tagId);

            if (chip) {
              chip.name = t.tagName;
            }
            this.toastr.success('Renamed tag successfully');
          },
          error: (response: HttpErrorResponse) => { console.error(response); this.toastr.error(response.error.message ?? 'Failed to rename tag') }
        });
    }
  }

  deleteTags() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const message = 'Are you sure you want to delete the selected tags?';
      const options = ConfirmDialogComponent.GetOptions('confirm-delete-dialog', 'Delete Tags', message, true);
      this.confirmModalRef = this.modalService.show(ConfirmDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogHiddenSubscription = this.modalService.onHidden
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
        .subscribe({
          next: () => {
            this.toastr.success(`Deleted ${chip.name} successfully`);
            this.tagChips.splice(this.tagChips.indexOf(chip), 1);
          },
          error: (response: HttpErrorResponse) => { console.error(response); this.toastr.error(response.error.message ?? `Failed to delete ${chip.name}`) }
        });
    }
  }

  copyTag() {
    const chips = this.getSelectedChips();

    if (chips.length) {
      const chip = chips[0];
      const tagType = chip.ownerId ? 'P' : 'S';
      const showRadios = User.isAdmin(this.currentUser) || User.isContributer(this.currentUser);       
      const message = 'Please enter a name for copied tag to be created.';
      const options = CopyTagDialogComponent.GetOptions('tag-copy-dialog', 'Copied Tag Name', 'Tag Name', message, chip.name, tagType, showRadios);
      this.copyTagModalRef = this.modalService.show(CopyTagDialogComponent, options);

      this.clearDialogSubscription();
      this.dialogHiddenSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.copyTagModalRef.content.okClicked) {
            const c = this.copyTagModalRef.content;
            
            if (c.tagName && c.tagName.trim() && (c.tagName.toUpperCase() != chip.name.toUpperCase() || c.tagType != tagType)) {
              const ownerId = c.tagType == 'S' ? null : this.currentUser.userId;
              this.copyTagSubmit(chip.id, c.tagName, ownerId);
              chip.selected = false;
            }
          }
        });
    }
  }

  private copyTagSubmit(sourceTagId: number, copyTagName: string, ownerId: number|null) {

    if (sourceTagId > 0 && copyTagName) {
      this.tagService.copyTag(sourceTagId, copyTagName, ownerId)
        .subscribe({
          next: (newTag) => {
            const chip = this.tagToChip(newTag);
            chip.selected = true;
            this.tagChips.push(chip);
            this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
            this.toastr.success('Copied tag successfully');            
          },
          error: (response: HttpErrorResponse) => { console.error(response); this.toastr.error(response.error.message ?? 'Failed to copy tag') }
        });      
    }
  }

  combineTags() {
    const chips = this.selectedTagChips;

    if (chips.length) {      
      const chip = chips[0];
      const tagType = chip.ownerId ? 'P' : 'S';
      const showRadios = User.isAdmin(this.currentUser) || User.isContributer(this.currentUser);       
      const message = 'Please enter a name for combined tag to be created.';
      const options = CopyTagDialogComponent.GetOptions('tag-combine-dialog', 'Combined Tag Name', 'Tag Name', message, chip.name, tagType, showRadios);
      this.copyTagModalRef = this.modalService.show(CopyTagDialogComponent, options);
      
      this.clearDialogSubscription();
      this.dialogHiddenSubscription = this.modalService.onHidden
        .subscribe(() => {
          if (this.copyTagModalRef.content.okClicked) {
            const c = this.copyTagModalRef.content;
            
            if (c.tagName && c.tagName.trim()) {
              const ownerId = c.tagType == 'S' ? null : this.currentUser.userId;
              this.combineTagsSubmit(chips.map(c => c.id), c.tagName, ownerId);
            }
          }
        });
    }
  }

  private combineTagsSubmit(sourceTagIds: number[], combinedTagName: string, ownerId: number|null) {

    if (sourceTagIds && sourceTagIds.length && combinedTagName && combinedTagName.length) {

      this.tagService.mergeTags(sourceTagIds, combinedTagName, ownerId)
        .subscribe({
          next: (newTag) => {
            sourceTagIds.forEach(id => this.tagChips.splice(this.tagChips.findIndex(c => c.id === id), 1));
            const chip = this.tagToChip(newTag);
            chip.selected = true;
            this.tagChips.push(chip);
            this.tagChips = this.tagChips.sort((a, b) => a.name < b.name ? - 1 : 1);
            this.toastr.success('Combined tags successfully');            
          },
          error: (response: HttpErrorResponse) => { console.error(response); this.toastr.error(response.error.message ?? 'Failed to combine tags') }
        });
    }
  }

  private clearDialogSubscription(): void {
    if (this.dialogHiddenSubscription) {
      this.dialogHiddenSubscription.unsubscribe();
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
    chip.ownerId = tag.ownerId;
    chip.ownerUsername = tag.ownerUsername;
    chip.count = tag.photoCount > 0 ? tag.photoCount : 0;
    chip.color = tag.tagColor || TagChip.defaultColor;
    chip.borderColor = RGB_Log_Shade(-.7, chip.color);
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
        const divider: TagChip = {
          name: letter,
          id: -1,
          isDivider: true,
          count: 0,
          selected: false
        };
        dividedChips.splice(dividedChips.length - 1, 0, divider);
      }
      prevLetter = letter;
    });

    return dividedChips;
  }
}
