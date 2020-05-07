import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { FileUploader } from 'ng2-file-upload';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent implements OnInit {

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;

  constructor(
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private modalService: BsModalService) {

      this.uploader = new FileUploader({ url: `${environment.apiUrl}/upload/imageUpload`, itemAlias: 'files' });

      this.hasBaseDropZoneOver = false;
      this.hasAnotherDropZoneOver = false;
      this.response = '';
      this.uploader.response.subscribe( res => this.response = res );
    }

  ngOnInit() {
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onCompleteItem = (item: any, response: any, status: any, headers: any) => {
         console.log('Uploaded file succeeded', item, status, response);
    };
  }

  onSubmit() {

  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
}
