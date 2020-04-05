import { Component, OnInit } from '@angular/core';
import { TagService, SearchService } from '../services';
import { TagChip, Tag } from '../models';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-tags',
  templateUrl: './tags.component.html',
  styleUrls: ['./tags.component.css']
})
export class TagsComponent implements OnInit {
  tagChips: TagChip[] = [];
  selectedTags: TagChip[] = [];

  constructor(private tagService: TagService,
    private router: Router,
    private searchService: SearchService) { }

  ngOnInit() {
    this.searchService.setHidden(false);

    this.tagService.getTags()
      .pipe(map(tags => this.tagsToChips(tags)))
      .subscribe((chips => this.tagChips = this.insertIndexDividers(chips)));

    this.searchService.getKeywords()
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
  }

  select(chip: TagChip) {
    chip.selected = !chip.selected;

    if (chip.selected) {
      this.selectedTags.push(chip);
    }
    else {
      this.selectedTags = this.selectedTags.filter(c => c.name !== c.name);
    }
    // this.router.navigate(['/photos', chip.name]);
  }

  matchesFilter(tagName: string): boolean {
    return true;
  }

  addTag() {

  }

  deleteTags() {

  }

  renameTag() {

  }

  copyTag() {

  }

  combineTags() {

  }

  clearSelections() {
    this.tagChips.forEach(chip => chip.selected = false);
    this.selectedTags = [];
  }

  private tagsToChips(photos: Tag[]): TagChip[] {
    const chips = new Array<TagChip>();
    photos.forEach(tag => chips.push(this.tagToChip(tag)));
    return chips;
  }

  private tagToChip(tag: Tag): TagChip {
    const chip = new TagChip();
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
        tagChips.splice(index, 0, {name: letter, isDivider: true, count: 0, selected: false}); // insert divider
      }
      prevLetter = letter;
    });

    return tagChips;
  }
}
