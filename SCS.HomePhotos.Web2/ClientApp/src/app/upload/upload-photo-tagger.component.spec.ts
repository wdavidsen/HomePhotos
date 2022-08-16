import { HttpClientModule } from '@angular/common/http';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { BsModalRef, BsModalService, ModalModule } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { of } from 'rxjs';
import { Tag } from '../models';
import { TagService } from '../services';
import { UploadPhotoTaggerComponent } from './upload-photo-tagger.component';

describe('UploadPhotoTaggerComponent', () => {
  let component: UploadPhotoTaggerComponent;
  let fixture: ComponentFixture<UploadPhotoTaggerComponent>;
  let mockToastr, mockTagService, mockModalService;

  const tags: Tag[] = [
    { tagId: 1, tagName: 'A Tag', photoCount: null },
    { tagId: 2, tagName: 'B Tag', photoCount: null },
    { tagId: 3, tagName: 'C Tag', photoCount: null }
  ];

  beforeEach(async(() => {
    mockToastr = jasmine.createSpyObj(['success', 'error']);
    mockModalService = jasmine.createSpyObj(['show', 'hide']);
    mockTagService = jasmine.createSpyObj(['getTags']);

    mockTagService.getTags.and.returnValue(of(tags));

    TestBed.configureTestingModule({
      declarations: [ UploadPhotoTaggerComponent ],
      imports: [HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      providers: [
        { provide: TagService, useValue: mockTagService },
        { provide: BsModalRef },
        { provide: BsModalService, useValue: mockModalService },
        { provide: ToastrService, useValue: mockToastr }
      ],
      schemas: [NO_ERRORS_SCHEMA],
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadPhotoTaggerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize', () => {
    expect(component.allTags).toBeTruthy();
    expect(component.allTags.length).toBe(3);
  });
});
