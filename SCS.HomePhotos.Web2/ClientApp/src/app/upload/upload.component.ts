import { Component, OnInit, ViewChild } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef, ModalDirective } from 'ngx-bootstrap/modal';
import { FileUploader, FileItem } from 'ng2-file-upload';
import { environment } from '../../environments/environment';
import { TagState, User } from '../models';
import { AuthService } from '../services';
import { PhotoTaggerComponent } from '../photos/photo-tagger.component';

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

  private tagList_shared: TagState[] = [];
  private tagList_personal: TagState[] = [];
  private currentUser: User;

  constructor(
    private authenticationService: AuthService,
    private toastr: ToastrService,
    private modalService: BsModalService) {

      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
      });

      const options = {
        url: `${environment.apiUrl}/upload/imageUpload`,
        itemAlias: 'files',
        allowedMimeType: ['image/jpeg', 'image/png'],
        parametersBeforeFiles: true,
        authToken: `Bearer ${authenticationService.getJwtToken()}`
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
      tagStates_shared: this.tagList_shared,
      tagStates_personal: this.tagList_personal,
      caller: 'PhotoUpload'
    };
    this.taggerModalRef = this.modalService.show(PhotoTaggerComponent, {initialState});
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
    const list1 = this.tagList_shared.length ? this.tagList_shared      
      .filter(ts => ts.checked)
      .map(ts => `0^${ts.tagName}`) : [];

    const list2 = this.tagList_personal.length ? this.tagList_personal      
      .filter(ts => ts.checked)
      .map(ts => `${this.currentUser.userId}^${ts.tagName}`) : [];

    const listFinal = list1.concat(list2).join(',');
    const data = { tagList: listFinal };

    this.uploader.options.additionalParameter = data;
    this.uploader.uploadAll();
  }
}
