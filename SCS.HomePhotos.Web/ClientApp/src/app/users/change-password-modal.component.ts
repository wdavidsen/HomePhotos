import { OnInit, Component } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AccountService, UserService } from '../services';
import { Observable } from 'rxjs';
import { ToastrService } from 'ngx-toastr';

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

        let result: Observable<any>;
        const username = this.f.username.value;
        const currentPassword = this.f.currentPassword.value;
        const newPassword = this.f.newPassword.value;
        const newPasswordCompare = this.f.newPasswordCompare.value;

        if (this.adminMode) {
            result = this.userService.changePassword(username, currentPassword, newPassword, newPasswordCompare);
        }
        else {
            result = this.accountService.changePassword(currentPassword, newPassword, newPasswordCompare);
        }

        result.subscribe(
            () => {
                this.toastr.success('Successfully changed password');
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
