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
    userName: string;
    changePasswordForm: FormGroup;

    constructor(
        public bsModalRef: BsModalRef,
        private formBuilder: FormBuilder,
        private accountService: AccountService,
        private userService: UserService,
        private toastr: ToastrService) {}

    ngOnInit() {
        this.changePasswordForm = this.formBuilder.group({
            username: [this.userName, Validators.required],
            currentPassword: ['', [Validators.required, Validators.minLength(8)]],
            newPassword: ['', [Validators.required, Validators.minLength(8)]],
            newPasswordCompare: ['', [Validators.required, Validators.minLength(8)]]
        });
    }

    onSubmit() {
        if (!this.changePasswordForm.valid) {
            return;
        }
        const changeInfo: PasswordChange = {
            userName: this.f.username.value,
            currentPassword: this.f.currentPassword.value,
            newPassword: this.f.newPassword.value,
            newPasswordCompare: this.f.newPasswordCompare.value
        };

        let result: Observable<any>;
        result = (this.adminMode) ? this.userService.changePassword(changeInfo) : this.accountService.changePassword(changeInfo);

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
