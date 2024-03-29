import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

import { AuthService, UserSettingsService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ChangePasswordModalComponent } from '../account/change-password-modal.component';
import { Subscription } from 'rxjs';
import { PasswordChange } from '../models';
import { HttpErrorResponse } from '@angular/common/http';

@Component({ 
    selector: "login-form",
    templateUrl: 'login.component.html' 
})
export class LoginComponent implements OnInit, OnDestroy {
    loginForm: UntypedFormGroup;
    loading = false;
    submitted = false;
    returnUrl: string;
    changePasswordModalRef: BsModalRef;

    private dialogSubscription: Subscription;
    private loginSubscription: Subscription;
    private loginWithPasswordChangeSubscription: Subscription;

    constructor(
        private formBuilder: UntypedFormBuilder,
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthService,
        private userSettingsService: UserSettingsService,
        private toastr: ToastrService,
        private modalService: BsModalService,
        public bsModalRef: BsModalRef)
    {
        if (this.authenticationService.currentUserValue) {
            authenticationService.logout();
        }
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
        this.loginForm = this.formBuilder.group({
            username: ['', Validators.required],
            password: ['', Validators.required]
        });

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

        this.login(this.f.username.value, this.f.password.value);
    }

    public login(username: string, password: string) {
        this.loading = true;
        this.loginSubscription = this.authenticationService.login(username, password)
            .subscribe({
                next: () => { 
                    this.userSettingsService.refreshCache();
                    this.toastr.success('Sign-in successful');
                    this.router.navigate([this.returnUrl]);
                    this.loading = false;
                },
                error: (response: HttpErrorResponse) => {
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
                }
            });
    }

    public loginWithPasswordChange(username: string, password: string) {
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
              const changeForm = <UntypedFormGroup>this.changePasswordModalRef.content.changePasswordForm;
              changeInfo.currentPassword = password;
              changeInfo.newPassword = changeForm.get('newPassword').value;
              changeInfo.newPasswordCompare = changeForm.get('newPasswordCompare').value;

              this.loginWithPasswordChangeSubscription = this.authenticationService.loginWithPasswordChange(changeInfo)
                .subscribe({
                    next: () => {
                        this.toastr.success('Sign-in with password change successful');
                        this.router.navigate([this.returnUrl]);
                        this.loading = false;
                    },
                    error: (response: HttpErrorResponse) => {
                        if (response.status > 99 && response.status < 600) {
                            console.error(response.error.error);
                            switch (response.error.error.id) {
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
                    }
                });
          });
      }
}
