import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AccountService, AuthService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { MustMatch } from '../validators/must-match.validator';

@Component({ templateUrl: 'register.component.html' })
export class RegisterComponent implements OnInit {
    registerForm: FormGroup;
    loading = false;
    submitted = false;

    constructor (
        private formBuilder: FormBuilder,
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
        this.authenticationService.loadCsrfToken().subscribe();
        this.registerForm = this.formBuilder.group({
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            username: ['', Validators.required],
            password: ['', [Validators.required, Validators.minLength(8)]],
            passwordCompare: ['', [Validators.required, Validators.minLength(8)]]
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
            .subscribe(
                () => {
                    this.toastr.success('Registration successful');
                    this.router.navigate(['/register-success']);
                },
                response => {
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
                });
    }
}
