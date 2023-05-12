import { Component, ViewChild, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';

import { AccountService, AuthService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { MustMatch } from '../validators/must-match.validator';
import { HttpErrorResponse } from '@angular/common/http';
import { PasswordRequirements, RegisteredInfo } from '../models';
import { LoginComponent } from '../login/login.component';

@Component({ templateUrl: 'register.component.html' })
export class RegisterComponent implements OnInit {
    registerForm: UntypedFormGroup;
    loading = false;
    submitted = false;
    passwordReq = new PasswordRequirements();
    passwordReqHelp = '';

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
        this.authenticationService.getPasswordRequirements()
            .subscribe({
                next: (req) => {      
                    this.passwordReq = req;              
                    this.passwordReqHelp = `(${req.minLength}+ characters required; at leaset ${req.digits} number, ${req.specialCharacters} special character & ${req.uppercaseCharacters} uppercase character)`;
                    this.setupForm(req);
                },
                error: (response: HttpErrorResponse) => console.error(response)
        });

        this.setupForm(this.passwordReq);
    }

    setupForm(passReq: PasswordRequirements) {
        this.registerForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            username: ['', Validators.required],
            emailAddress: ['', Validators.email],
            password: ['', [Validators.required, Validators.minLength(passReq?.minLength ?? 8)]],
            passwordCompare: ['', [Validators.required]]
        },
        {
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
                    this.loading = false;
                    this.toastr.success('Registration successful');

                    if (nextStep.autoApproved) {
                        this.loginForm.login(this.f.username.value, this.f.password.value);
                    }
                    else {
                        this.router.navigate(['/register-success']);
                    }                    
                },
                error: (response: HttpErrorResponse) => { 
                    this.loading = false;                        
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
                }
            });
    }
}
