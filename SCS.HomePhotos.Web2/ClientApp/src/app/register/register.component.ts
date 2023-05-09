import { Component, ViewChild, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

import { AccountService, AuthService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { MustMatch } from '../validators/must-match.validator';
import { HttpErrorResponse } from '@angular/common/http';
import { RegisteredInfo } from '../models';
import { LoginComponent } from '../login/login.component';

@Component({ templateUrl: 'register.component.html' })
export class RegisterComponent implements OnInit {
    registerForm: UntypedFormGroup;
    loading = false;
    submitted = false;    
    @ViewChild(LoginComponent) loginForm: LoginComponent;

    constructor (
        private formBuilder: UntypedFormBuilder,
        private router: Router,
        private authenticationService: AuthService,
        private accountService: AccountService,
        private toastr: ToastrService) {

        // redirect to home if already logged in
        if (this.authenticationService.currentUserValue) {
            this.router.navigate(['/']);
        }
    }

    ngOnInit() {
        this.registerForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            username: ['', Validators.required],
            password: ['', [Validators.required]],
            passwordCompare: ['', [Validators.required]]
        }, {
            validator: MustMatch('password', 'passwordCompare')
        });
    }

    // convenience getter for easy access to form fields
    get f() { return this.registerForm.controls; }

    onSubmit() {
        this.submitted = true;

        // stop here if form is invalid
        if (this.registerForm.invalid) {
            return;
        }

        this.loading = true;
        this.accountService.register(this.registerForm.value)
            .subscribe({
                next: (nextStep: RegisteredInfo) => {
                    this.toastr.success('Registration successful');

                    if (nextStep.autoApproved) {
                        this.loginForm.login(this.f.username.value, this.f.password.value);
                    }
                    else {
                        this.router.navigate(['/register-success']);
                    }                    
                },
                error: (response: HttpErrorResponse) => {                                        
                    if (response.error && response.error.id) {
                        switch (response.error.id) {
                            case 'UserNameTaken':
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
                        this.toastr.error('Server unreachable');
                    }
                },
                complete: () => this.loading = false
            });
    }
}
