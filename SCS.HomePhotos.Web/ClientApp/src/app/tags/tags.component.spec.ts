import { HttpClientModule } from '@angular/common/http';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { IndividualConfig, ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { Tag, TagChip } from '../models';
import { TagService } from '../services';

import { TagsComponent } from './tags.component';

describe('TagsComponent', () => {
  let component: TagsComponent;
  let fixture: ComponentFixture<TagsComponent>;
  let mockToastr, mockTagService;

  const tags: Tag[] = [
    { tagId: 1, tagName: 'a tag1', photoCount: 5 },
    { tagId: 2, tagName: 'b tag1', photoCount: 10 },
    { tagId: 3, tagName: 'b tag2', photoCount: 15 }
  ];

  const tagChips: TagChip[] = [
    { id: 1, name: 'a tag1', isDivider: false, selected: false, count: 5 },
    { id: 0, name: 'B', isDivider: true, selected: false, count: null },
    { id: 2, name: 'b tag1', isDivider: false, selected: false, count: 10 },
    { id: 3, name: 'b tag2', isDivider: false, selected: false, count: 15 }
  ];

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockTagService = jasmine.createSpyObj(['getTags', 'searchTags', 'addTag', 'updateTag', 'deleteTag', 'copyTag', 'mergeTags', ]);

    mockTagService.getTags.and.returnValue(of(tags));

    TestBed.configureTestingModule({
      declarations: [TagsComponent],
      imports: [HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      // schemas: [NO_ERRORS_SCHEMA],
      providers: [
        TagService,
        BsModalService,
        { provide: ToastrService, useValue: mockToastr },
        { provide: TagService, useValue: mockTagService }
      ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TagsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.tagChips.length).toBe(3);
    expect(mockTagService.getTags).toHaveBeenCalledTimes(1);
  });

  it('should render tags', () => {
    component.tagChips = tagChips;
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.tag:not(.tag-divider)').length).toBe(3);
  });

  it('should render tags with dividers', () => {
    const tagList: Tag[] = [];

    for (let i = 65; i < 91; i++) {
      tagList.push({ tagId: i, tagName: `${String.fromCharCode(i)} tag1`, photoCount: 5 });
      tagList.push({ tagId: i, tagName: `${String.fromCharCode(i)} tag2`,  photoCount: 5 });
    }

    mockTagService.getTags.and.returnValue(of(tagList));
    component.ngOnInit();
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.tag-divider').length).toBe(26);
  });

  it('should render tag text', () => {
    component.tagChips = tagChips;
    fixture.detectChanges();

    expect(fixture.nativeElement.querySelectorAll('.tag')[2].innerHTML).toContain('b tag1');
  });

  it('should render divider text', () => {
    component.tagChips = tagChips;
    fixture.detectChanges();

    // expect(fixture.debugElement.query(By.css('.tag-divider')).nativeElement.textContent).toContain('B');
    expect(fixture.nativeElement.querySelector('.tag-divider').innerHTML).toContain('B');
  });

  it('should select a tag', () => {
    component.tagChips = tagChips;
    component.organizeMode = true;
    fixture.detectChanges();

    component.select(tagChips[2]);
    expect(tagChips[2].selected).toBe(true);

    component.select(tagChips[2]);
    expect(tagChips[2].selected).toBe(false);
  });

  it('should get selected tags', () => {
    component.tagChips = tagChips;
    component.organizeMode = true;
    fixture.detectChanges();

    component.select(tagChips[0]);
    component.select(tagChips[2]);

    const selectedCount = component.getSelectedChips().length;
    expect(selectedCount).toBe(2);
  });

  it('should clear selected tags', () => {
    component.tagChips = tagChips;
    component.organizeMode = true;
    fixture.detectChanges();

    component.select(tagChips[0]);
    component.select(tagChips[2]);

    component.clearSelections();
    const selectedCount = component.getSelectedChips().length;

    expect(selectedCount).toBe(0);
  });
});
