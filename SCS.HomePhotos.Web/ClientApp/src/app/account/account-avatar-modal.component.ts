import { OnInit, Component, ViewChild, OnDestroy, ElementRef } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup } from '@angular/forms';
import { AccountService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { PasswordChange } from '../models';
import { CropperComponent } from 'angular-cropperjs';

@Component({
    selector: 'app-account-avatar-modal',
    templateUrl: './account-avatar-modal.component.html',
    styleUrls: ['./account-avatar-modal.component.css']
  })

export class AccountAvatardModalComponent implements OnInit, OnDestroy {
    @ViewChild('fileInput', {static: true})
    fileInput: ElementRef;

    @ViewChild('angularCropper', {static: false})
    angularCropper: CropperComponent;

    title: string;
    loginMode: boolean;
    userName: string;
    changePasswordForm: FormGroup;
    changeInfo: PasswordChange;
    message: string;
    okText = 'Update';
    newAvatarImage: string = null;

    ready = false;
    imageUrl: any = '/assets/images/avatar-placeholder.png';
    selectedfile: File = null;
    cropperOptions = {
      aspectRatio: 1,
      responsive: false,
      rotatable: true,
      scalable: true,
      zoomable: true,
      zoomOnTouch: true,
      movable: true,
      cropBoxMovable: true,
      checkOrientation: true,
      preview: '#avatarPreviewImage'
    };

    constructor(
        public bsModalRef: BsModalRef,
        private accountService: AccountService,
        private toastr: ToastrService) {}

    ngOnInit() {
      this.fileInput.nativeElement.onchange = () => {
        this.selectedfile = this.fileInput.nativeElement.files[0];
        const reader = new FileReader();
        reader.addEventListener('load', () => this.imageUrl = reader.result, false);
        reader.readAsDataURL(this.selectedfile);
      };
    }

    ngOnDestroy(): void {
      if (this.angularCropper.cropper) {
        this.angularCropper.cropper.destroy();
      }
    }

    onSubmit() {
      if (this.angularCropper.cropper) {
        const cropper = this.angularCropper.cropper;
        // const imageInfo = cropper.getImageData();
        const canvas = cropper.getCroppedCanvas();

        canvas.toBlob((blob) => {
          let file: File;
          const temp: any = blob;
          temp.name = this.selectedfile.name;
          temp.lastModifiedDate = new Date();
          file = temp;

          this.accountService.updateAvatar(file)
            .subscribe(
              (data) => {
                this.newAvatarImage = data.avatarImage;
                this.toastr.success('Successfully saved profile picture.');
                this.bsModalRef.hide();
              },
              () => {
                this.newAvatarImage = null;
                this.toastr.error('Failed to save profile picture.');
              });
        });
      }
    }

    zoom(direction: String): void {
      if (this.angularCropper.cropper) {
        const cropper = this.angularCropper.cropper;

        if (direction === 'in') {
          cropper.zoom(0.1);
        }
        else {
          cropper.zoom(-0.1);
        }
      }
    }

    rotate(direction) {
      if (this.angularCropper.cropper) {
        const cropper = this.angularCropper.cropper;

        if (direction === 'cw') {
          cropper.rotate(90);
        }
        else {
          cropper.rotate(-90);
        }
      }
    }
}
