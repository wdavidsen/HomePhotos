import { Component, OnInit, OnDestroy } from '@angular/core';
import { TagService, SearchService, OrganizeService } from '../services';
import { TagChip, Tag } from '../models';
import { map, tap } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit, OnDestroy {
  tagChips: TagChip[] = [];
  organizeMode = false;

  private searchSubscription: Subscription;
  private organizeSubscription: Subscription;

  constructor(private tagService: TagService,
    private router: Router,
    private searchService: SearchService,
    private organizeService: OrganizeService,
    private toastr: ToastrService) { }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.tagService.getTags()
      // .pipe(tap(tags => tags.length ? this.toastr.success(`Loaded ${tags.length} tags`) : this.toastr.warning(`No tags available`)))
      .pipe(map(tags => this.tagsToChips(tags)))
      .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));

    this.searchSubscription = this.searchService.getKeywords()
      .subscribe(keywords => {
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
    const tag: Tag = {
      tagId: 0,
      tagName: 'foo'
    };

    this.tagService.addTag(tag)
      .subscribe(t => {
        this.tagChips.push(this.tagToChip(t));
      },
      err => this.toastr.error(err.message)
    );
  }

  renameTag() {
    const tag: Tag = {
      tagId: 1,
      tagName: 'foo'
    };

    this.tagService.updateTag(tag)
      .subscribe(t => {
        const chip = this.tagChips.find(c => c.id === t.tagId);

        if (chip) {
          chip.name = t.tagName;
        }
      },
      err => this.toastr.error(err.message)
    );
  }

  deleteTags() {
    this.getSelectedChips().forEach(chip => {
      this.tagService.deleteTag(chip.id)
        .subscribe(() => {
            this.toastr.success(`Deleted ${chip.name} successfully`);
            this.tagChips.splice(this.tagChips.indexOf(chip), 1);
          },
          err => this.toastr.error(err.message)
        );
    });
  }

  copyTag() {

  }

  combineTags() {

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
    chip.count = tag.photoCount;
    return chip;
  }

  private insertIndexDividers(tagChips: TagChip[]): TagChip[] {
    if (tagChips.length < 30) {
        return tagChips;
    }
    const idexes = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];

    let letter = '';
    let prevLetter = '';

    tagChips.forEach((chip, index) => {
      letter = chip.name.substring(0, 1).toUpperCase();

      if (letter !== prevLetter && idexes.indexOf(letter) >= 0) {
        tagChips.splice(index, 0, {name: letter, id: -1, isDivider: true, count: 0, selected: false}); // insert divider
      }
      prevLetter = letter;
    });

    return tagChips;
  }
}
