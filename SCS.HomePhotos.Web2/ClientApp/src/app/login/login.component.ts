import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { AuthService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ChangePasswordModalComponent } from '../account/change-password-modal.component';
import { Subscription } from 'rxjs';
import { PasswordChange } from '../models';

@Component({ templateUrl: 'login.component.html' })
export class LoginComponent implements OnInit, OnDestroy {
    loginForm: FormGroup;
    loading = false;
    submitted = false;
    returnUrl?: string;
    changePasswordModalRef?: BsModalRef;

    private dialogSubscription?: Subscription;
    private loginSubscription?: Subscription;
    private loginWithPasswordChangeSubscription?: Subscription;

    constructor(
        private formBuilder: FormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthService,
        private toastr: ToastrService,
        private modalService: BsModalService,
        public bsModalRef: BsModalRef)
    {
        if (this.authenticationService.currentUserValue) {
            authenticationService.logout();
        }

        this.loginForm = this.formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required]
        });
    }
    ngOnDestroy(): void {
        if (this.dialogSubscription) {
            this.dialogSubscription.unsubscribe();
        }
        if (this.loginSubscription) {
            this.loginSubscription.unsubscribe();
        }
        if (this.loginWithPasswordChangeSubscription) {
            this.loginWithPasswordChangeSubscription.unsubscribe();
        }
    }

    ngOnInit() {        
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    // convenience getter for easy access to form fields
    get f() { return this.loginForm.controls; }

    onSubmit() {
        this.submitted = true;

        // stop here if form is invalid
        if (this.loginForm.invalid) {
            return;
        }

        const username = this.f.username.value;
        const password = this.f.password.value;

        this.loading = true;
        this.loginSubscription = this.authenticationService.login(username, password)
            .subscribe(
                () => {
                    this.toastr.success('Sign-in successful');
                    this.router.navigate([this.returnUrl]);
                    this.loading = false;
                },
                response => {
                    if (response.status > 99 && response.status < 600) {
                        switch (response.error.id) {
                            case 'PasswordChangeNeeded':
                                this.toastr.warning(response.error.message);
                                this.loginWithPasswordChange(username, password);
                                break;
                            case 'LoginFailed':
                                this.toastr.warning(response.error.message);
                                break;
                            default:
                                this.toastr.error('Sign-in failed');
                                break;
                        }
                    }
                    else {
                        this.toastr.error('Server unreachable');
                    }
                    this.loading = false;
                });
    }

    private loginWithPasswordChange(username: string, password: string) {
        const changeInfo: PasswordChange = {
            userName: username,
            currentPassword: password,
            newPassword: '',
            newPasswordCompare: ''
        };
        const initialState = {
            title: 'Change Password',
            okText: 'OK',
            message: 'Please change your password to continue.',
            loginMode: true,
            userName: username,
            changeInfo: changeInfo
        };
        this.changePasswordModalRef = this.modalService.show(ChangePasswordModalComponent, {initialState});

        this.dialogSubscription = this.modalService.onHidden
          .subscribe(() => {
              const changeForm = <FormGroup>this.changePasswordModalRef?.content.changePasswordForm;
              changeInfo.currentPassword = password;
              changeInfo.newPassword = changeForm.get('newPassword')?.value;
              changeInfo.newPasswordCompare = changeForm.get('newPasswordCompare')?.value;

              this.loginWithPasswordChangeSubscription = this.authenticationService.loginWithPasswordChange(changeInfo)
                .subscribe(
                    () => {
                        this.toastr.success('Sign-in with password change successful');
                        this.router.navigate([this.returnUrl]);
                        this.loading = false;
                    },
                    response => {
                        if (response.status > 99 && response.status < 600) {
                            console.error(response.error);
                            switch (response.error.id) {
                                case 'LoginFailed':
                                    this.toastr.warning(response.error.message);
                                    break;
                                default:
                                    this.toastr.error('Sign-in with password change failed');
                                    break;
                            }
                        }
                        else {
                            this.toastr.error('Server unreachable');
                        }
                        this.loading = false;
                    });
          });
      }
}
