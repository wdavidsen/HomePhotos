import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, FormGroup, Validators } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { ToastrService } from "ngx-toastr";
import { Subscription } from "rxjs";
import { AuthService } from "../services";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit, OnDestroy {
  loginForm: FormGroup;
    loading = false;
    submitted = false;
    returnUrl: string;
    //changePasswordModalRef: BsModalRef;

    private dialogSubscription: Subscription;
    private loginSubscription: Subscription;
    private loginWithPasswordChangeSubscription: Subscription;
    
  constructor(
    private authenticationService: AuthService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private toastr: ToastrService,) { }

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required]
    });

    // get return url from route parameters or default to '/'
    this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
  }

  ngOnDestroy() {
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
                            //this.loginWithPasswordChange(username, password);
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
}
