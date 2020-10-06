import { HttpClientModule } from '@angular/common/http';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { RouterTestingModule } from '@angular/router/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { PhotosService } from '../services';

import { PhotosComponent } from './photos.component';
import { LocalStorageService } from '../services/local-storage.service';
import { IndividualConfig, ToastrService } from 'ngx-toastr';
import { BsModalService, ModalModule } from 'ngx-bootstrap/modal';

const toastrService = {
  success: (
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ) => {},
  error: (
    message?: string,
    title?: string,
    override?: Partial<IndividualConfig>
  ) => {},
};

describe('PhotosComponent', () => {
  let component: PhotosComponent;
  let fixture: ComponentFixture<PhotosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PhotosComponent ],
      imports: [ReactiveFormsModule, HttpClientModule, RouterTestingModule, ModalModule.forRoot()],
      providers: [
        PhotosService,
        LocalStorageService,
        BsModalService,
        { provide: ToastrService, useValue: toastrService }]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PhotosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
