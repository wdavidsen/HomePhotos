import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { UserService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { PasswordChange } from '../models';
import { MustMatch } from '../validators/must-match.validator';

@Component({
    selector: 'app-reset-password-modal',
    templateUrl: './reset-password-modal.component.html'
  })

export class ResetPasswordModalComponent implements OnInit {
    title: string;
    userId: number;
    userName: string;
    resetPasswordForm: FormGroup;
    resetInfo: PasswordChange;
    message: string;
    okText = 'Reset Password';

    constructor(
        public bsModalRef: BsModalRef,
        private formBuilder: FormBuilder,
        private userService: UserService,
        private toastr: ToastrService) {}

      ngOnInit() {
        this.resetPasswordForm = this.formBuilder.group({
            username: [this.userName, [Validators.required]],
            newPassword: [null, [Validators.required, Validators.minLength(8)]],
            newPasswordCompare: [null, [Validators.required, Validators.minLength(8)]]
        }, {
            validator: MustMatch('newPassword', 'newPasswordCompare')
        });

        this.resetPasswordForm.get('username').disable();
    }

    onSubmit() {
        if (!this.resetPasswordForm.valid) {
            return;
        }
        this.resetInfo = {
            userName: this.f.username.value,
            currentPassword: null,
            newPassword: this.f.newPassword.value,
            newPasswordCompare: this.f.newPasswordCompare.value
        };

        this.userService.resetPassword(this.userId, this.resetInfo)
            .subscribe(() => {
                this.toastr.success('Successfully reset password');
                this.bsModalRef.hide();
            },
            response => {
                if (response.error && response.error.id) {
                    switch (response.error.id) {
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
                    this.toastr.error('Reset password failed');
                }
            }
        );
    }

    // convenience getter for easy access to form fields
    get f() { return this.resetPasswordForm.controls; }
}
