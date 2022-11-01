import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { UntypedFormGroup, UntypedFormBuilder, Validators } from '@angular/forms';
import { AccountService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { PasswordChange } from '../models';
import { MustMatch } from '../validators/must-match.validator';

@Component({
    selector: 'app-change-password-modal',
    templateUrl: './change-password-modal.component.html'
  })

export class ChangePasswordModalComponent implements OnInit {
    title: string;
    loginMode: boolean;
    userName: string;
    changePasswordForm: UntypedFormGroup;
    changeInfo: PasswordChange;
    message: string;
    okText = 'Change Password';

    constructor(
        public bsModalRef: BsModalRef,
        private formBuilder: UntypedFormBuilder,
        private accountService: AccountService,
        private toastr: ToastrService) {}

      ngOnInit() {
        this.changePasswordForm = this.formBuilder.group({
            username: [this.userName, [Validators.required]],
            currentPassword: [null, [Validators.required, Validators.minLength(8)]],
            newPassword: [null, [Validators.required, Validators.minLength(8)]],
            newPasswordCompare: [null, [Validators.required, Validators.minLength(8)]]
        }, {
            validator: MustMatch('newPassword', 'newPasswordCompare')
        });

      this.changePasswordForm.get('username').disable();

      if (this.loginMode) {
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
            return;
        }

        this.accountService.changePassword(this.changeInfo)
            .subscribe(() => {
                this.toastr.success('Successfully changed password');
                this.bsModalRef.hide();
            },
            response => {
                if (response.error && response.error.id) {
                  switch (response.error.id) {
                        case 'CurrentPasswordFailed':
                        case 'PasswordStrength':
                        case 'InvalidRequestPayload':
                            this.toastr.warning(response.error.message);
                            break;
                        default:
                            this.toastr.error(response.error.message);
                            break;
                    }
                }
                else {
                    this.toastr.error('Change password failed');
                }
            }
        );
    }

    // convenience getter for easy access to form fields
    get f() { return this.changePasswordForm.controls; }
}
