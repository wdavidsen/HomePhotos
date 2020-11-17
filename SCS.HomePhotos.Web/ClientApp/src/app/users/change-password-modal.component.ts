import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService, UserService } from '../services';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { PasswordChange } from '../models';

@Component({
    selector: 'app-modal-content',
    templateUrl: './change-password-modal.component.html'
  })

export class ModalContentComponent implements OnInit {
    title: string;
    adminMode: boolean;
    loginMode: boolean;
    userName: string;
    changePasswordForm: FormGroup;
    changeInfo: PasswordChange;
    message: string;
    closeText = 'Close';

    constructor(
        public bsModalRef: BsModalRef,
        private formBuilder: FormBuilder,
        private accountService: AccountService,
        private userService: UserService,
        private toastr: ToastrService) {}

      ngOnInit() {
        this.changePasswordForm = this.formBuilder.group({
            username: [this.userName, [Validators.required]],
            currentPassword: [null, [Validators.required, Validators.minLength(8)]],
            newPassword: [null, [Validators.required, Validators.minLength(8)]],
            newPasswordCompare: [null, [Validators.required, Validators.minLength(8)]]
        });

      if (this.loginMode) {
        this.changePasswordForm.get('username').disable();
        this.changePasswordForm.get('currentPassword').disable();
      }
    }

    onSubmit() {
        if (!this.changePasswordForm.valid) {
            return;
        }
        this.changeInfo = {
            userName: this.f.username.value,
            currentPassword: this.f.currentPassword.value,
            newPassword: this.f.newPassword.value,
            newPasswordCompare: this.f.newPasswordCompare.value
        };

        if (this.loginMode) {
            this.bsModalRef.hide();
        }

        let result: Observable<any>;
        result = (this.adminMode) ? this.userService.changePassword(this.changeInfo) : this.accountService.changePassword(this.changeInfo);

        result.subscribe(
            () => {
                this.toastr.success('Successfully changed password');
                this.bsModalRef.hide();
            },
            error => {
                console.error(error);
                this.toastr.success('Failed to change password');
            }
        );
    }

    // convenience getter for easy access to form fields
    get f() { return this.changePasswordForm.controls; }
}
