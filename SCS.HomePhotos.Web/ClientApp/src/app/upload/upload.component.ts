import { Component, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef, ModalDirective } from 'ngx-bootstrap/modal';
import { FileUploader, FileItem } from 'ng2-file-upload';
import { environment } from '../../environments/environment';
import { UploadPhotoTaggerComponent } from './upload-photo-tagger.component';
import { TagState, User } from '../models';
import { AuthenticationService } from '../services';

declare const loadImage: any;

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  @ViewChild('selectModal', {static: true})
  selectModal: ModalDirective;

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  taggerModalRef: BsModalRef;
  thumbnails: Array<string> = [];

  private tagList: TagState[] = [];
  private currentUser: User;

  constructor(
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private modalService: BsModalService) {

      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
      });

      const options = {
        url: `${environment.apiUrl}/upload/imageUpload`,
        itemAlias: 'files',
        allowedMimeType: ['image/jpeg', 'image/png'],
        parametersBeforeFiles: true,
        authToken: `Bearer ${this.currentUser.token}`
      };
      this.uploader = new FileUploader(options);

      this.hasBaseDropZoneOver = false;
      this.hasAnotherDropZoneOver = false;
      this.response = '';
      this.uploader.response.subscribe( res => this.response = res );
    }

  ngOnInit() {
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onCompleteItem = (item, response, status, headers) => {
        console.log('Uploaded file succeeded', item, status, response);
    };

    this.uploader.onWhenAddingFileFailed = (item, filter, options) => {

    };

    this.uploader.onAfterAddingAll = (items) => {
      items.forEach((fileItem: any, index: number) => this.createThumbnail(fileItem, index));
      this.selectModal.hide();
    };

    this.uploader.onCompleteAll = () => {
      this.toastr.info('Upload complete');
    };
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  selectImages() {
    this.selectModal.show();
  }

  uploadAll() {
    this.tagPhotos();
    this.modalService.onHidden
      .subscribe(() => {
        this.uploadFiles();
      });
  }

  cancelAll() {
    this.uploader.cancelAll();
  }

  removeItem(item: FileItem, index: number) {
    item.remove();
    this.thumbnails.splice(index, 1);
  }

  removeAll() {
    this.uploader.clearQueue();
    this.thumbnails = [];
  }

  tagPhotos() {
    const initialState = {
      title: 'Photo Tagger',
      tagStates: this.tagList
    };
    this.taggerModalRef = this.modalService.show(UploadPhotoTaggerComponent, {initialState});
  }

  private createThumbnail(fileItem: any, index: number) {
    const offset = this.thumbnails.length;
    const options = {
      maxWidth: 120,
      maxHeight: 120,
      orientation: true,
      canvas: true
    };
    loadImage(fileItem._file, (canvas) => {
      this.thumbnails[index + offset] = canvas.toDataURL();
    },
    options);
  }

  private uploadFiles() {
    const list = this.tagList.length ? this.tagList
      .filter(ts => ts.checked)
      .map(ts => ts.tagName).join(',') : null;

    const data = { tagList: list };

    this.uploader.options.additionalParameter = data;
    this.uploader.uploadAll();
  }
}
