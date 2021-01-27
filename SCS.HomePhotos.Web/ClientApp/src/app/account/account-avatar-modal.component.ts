import { OnInit, Component, ViewChild } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { PasswordChange } from '../models';
import { CropperComponent } from 'angular-cropperjs';

@Component({
    selector: 'app-account-avatar-modal',
    templateUrl: './account-avatar-modal.component.html'
  })

export class AccountAvatardModalComponent implements OnInit {

    @ViewChild('angularCropper', {static: true})
    angularCropper: CropperComponent;

    title: string;
    loginMode: boolean;
    userName: string;
    changePasswordForm: FormGroup;
    changeInfo: PasswordChange;
    message: string;
    okText = 'Change Picture';

    constructor(
        public bsModalRef: BsModalRef,
        private formBuilder: FormBuilder,
        private accountService: AccountService,
        private toastr: ToastrService) {}

      ngOnInit() {

    }

    onSubmit() {
    }
}
