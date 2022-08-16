import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of, Subject } from 'rxjs';
import { TriCheckState } from '../components/tri-check.component';
import { Tag, TagState } from '../models';
import { TagService } from '../services';
import { PhotoTaggerComponent } from './photo-tagger.component';


describe('PhotoTaggerComponent', () => {
    let component: PhotoTaggerComponent;
    let fixture: ComponentFixture<PhotoTaggerComponent>;
    let router;

    let mockToastr, mockTagService, mockModalRef;

    beforeEach(async(() => {
        mockToastr = jasmine.createSpyObj(['success', 'error', 'warning']);
        mockTagService = jasmine.createSpyObj(['updatePhotoTags', 'getPhototags', 'getTags']);
        mockModalRef = jasmine.createSpyObj(['hide']);

        const tags: Tag[] = [
            { tagId: 1, tagName: 'A Tag', photoCount: 5 },
            { tagId: 2, tagName: 'B Tag', photoCount: 5 },
            { tagId: 3, tagName: 'C Tag', photoCount: 5 }
        ];
        const tagStates: TagState[] = [
            { tagId: 2, tagName: 'B Tag', checked: true, indeterminate: false, allowIndeterminate: false },
            { tagId: 3, tagName: 'C Tag', checked: false, indeterminate: true, allowIndeterminate: true }
        ];

        mockTagService.getTags.and.returnValue(of(tags));
        mockTagService.getPhototags.and.returnValue(of(tagStates));

        TestBed.configureTestingModule({
            declarations: [PhotoTaggerComponent],
            imports: [FormsModule, RouterTestingModule],
            providers: [
              { provide: TagService, useValue: mockTagService },
              { provide: BsModalRef, useValue: mockModalRef},
              { provide: ToastrService, useValue: mockToastr }],
              schemas: [NO_ERRORS_SCHEMA]
          })
          .compileComponents();
    }));

    beforeEach(() => {
        fixture = TestBed.createComponent(PhotoTaggerComponent);
        component = fixture.componentInstance;

        router = TestBed.get(Router);
        // location = TestBed.get(Location);

        fixture.detectChanges();
        router.initialNavigation();
      });

      it('should create', () => {
        expect(component).toBeTruthy();
      });

      it('should initialize', () => {
        expect(component.allTags.length).toBe(3);
        expect(component.tagStates.length).toBe(2);
      });

      it('should save', () => {
        mockTagService.updatePhotoTags.and.returnValue(of(''));
        component.dirtyTags = ['B Tag'];
        fixture.detectChanges();

        component.saveTags();

        expect(mockTagService.updatePhotoTags).toHaveBeenCalledTimes(1);
        expect(mockToastr.success).toHaveBeenCalledTimes(1);
        expect(mockModalRef.hide).toHaveBeenCalledTimes(1);
        expect(component.dirtyTags.length).toBe(0);
      });

      it('should handle save error', () => {
        const sub = new Subject<any>();
        sub.error('some error');
        mockTagService.updatePhotoTags.and.returnValue(sub.asObservable());
        component.dirtyTags = ['B Tag'];
        fixture.detectChanges();

        component.saveTags();

        expect(mockTagService.updatePhotoTags).toHaveBeenCalledTimes(1);
        expect(mockToastr.error).toHaveBeenCalledTimes(1);
        expect(component.dirtyTags.length).toBe(1);
      });

      it('should add new tag', () => {
        component.selectedTag = 'D Tag';
        fixture.detectChanges();

        component.addTag();

        expect(component.tagStates.length).toBe(3);
        expect(component.dirtyTags.length).toBe(1);
        expect(component.selectedTag).toBe(null);
      });

      it('should not add existing tag', () => {
        component.selectedTag = 'C Tag';
        fixture.detectChanges();

        component.addTag();

        expect(mockToastr.warning).toHaveBeenCalledTimes(1);
        expect(component.tagStates.length).toBe(2);
      });

      it('should change tag state', () => {
        const idx = 0;
        const tagName = component.tagStates[idx].tagName;
        const event = new TriCheckState();
        event.checked = false;
        event.label = tagName;

        component.tagStateChanged(event);

        expect(component.tagStates[idx].tagName).toBe(tagName);
        expect(component.tagStates[idx].checked).toBe(false);
        expect(component.tagStates[idx].indeterminate).toBe(false);
        expect(component.dirtyTags[idx]).toBe(tagName);
      });

      it('should allow indeterminate state', () => {
        const idx = 1;
        const tagName = component.tagStates[idx].tagName;
        const event = new TriCheckState();
        event.indeterminate = true;
        event.checked = true;
        event.label = tagName;

        component.tagStateChanged(event);

        expect(component.tagStates[idx].tagName).toBe(tagName);
        expect(component.tagStates[idx].checked).toBe(true);
        expect(component.tagStates[idx].indeterminate).toBe(true);
        expect(component.dirtyTags).toContain(tagName);
      });

      it('should not allow indeterminate state', () => {
        const idx = 0;
        const tagName = component.tagStates[idx].tagName;
        const event = new TriCheckState();
        event.indeterminate = true;
        event.checked = true;
        event.label = tagName;

        component.tagStateChanged(event);

        expect(component.tagStates[idx].tagName).toBe(tagName);
        expect(component.tagStates[idx].checked).toBe(true);
        expect(component.tagStates[idx].indeterminate).toBe(false);
        expect(component.dirtyTags).toContain(tagName);
      });

    //   it('should render tags', () => {

    //     expect(fixture.nativeElement.querySelectorAll('.tag:not(.tag-divider)').length).toBe(3);
    //   });
});
