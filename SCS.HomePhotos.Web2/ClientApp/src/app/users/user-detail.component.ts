import { Component, OnInit } from '@angular/core';
import { UntypedFormGroup, Validators, UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService, AuthService } from '../services';
import { PasswordRequirements, User } from '../models';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ResetPasswordModalComponent } from './reset-password-modal.component';
import { HttpErrorResponse } from '@angular/common/http';
import { MustMatch } from '../validators/must-match.validator';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  private currentUser: User;
  user: User;
  userForm: UntypedFormGroup;
  roles = ['Reader', 'Contributor', 'Admin'];
  loading = false;
  submitted = false;
  resetPasswordModalRef: BsModalRef;
  passwordReq = new PasswordRequirements();
  passwordReqHelp = '';

  constructor(
    private route: ActivatedRoute,
    private authenticationService: AuthService,
    private router: Router,
    private formBuilder: UntypedFormBuilder,
    private userService: UserService,
    private toastr: ToastrService,
    private modalService: BsModalService) {
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
      });
    }

  // convenience getter for easy access to form fields
  get f() { return this.userForm.controls; }

  ngOnInit() {
    this.authenticationService.getPasswordRequirements()
      .subscribe({
        next: (req) => {
          this.passwordReq = req;
          this.passwordReqHelp = `(${req.minLength}+ characters required; at leaset ${req.digits} number, ${req.specialCharacters} special character & ${req.uppercaseCharacters} uppercase character)`;
          this.setupForm(this.passwordReq, this.user);
        },
        error: (response: HttpErrorResponse) => console.error(response)
      });

      this.user = new User();
      this.setupForm(this.passwordReq, this.user);

      this.route.paramMap.subscribe(params => {
          const userId = params.get('userId');
          console.log(`Received userId: ${userId}`);

          if (userId !== null && parseInt(userId, 10) > 0) {
              this.loading = true;

              this.userService.get(parseInt(userId, 10))
                .subscribe({
                  next: (data) => {
                    this.user = data;
                    this.setupForm(this.passwordReq, data);
                    this.loading = false;
                  },
                  error: (response: HttpErrorResponse) => {
                    console.error(response);
                    this.toastr.error(`Failed to find user`);
                    this.loading = false;
                  }
                });
          }
      });
  }

  onSubmit() {
    this.submitted = true;

    // stop here if form is invalid
    if (this.userForm.invalid) {
        return;
    }

    this.loading = true;
    const tempUser = this.formToUser();

    this.userService.save(tempUser)
      .subscribe({
        next: (user) => {
          this.loading = false;
          this.user = user;
          this.toastr.success(`User saved successfully`);          
          this.setupForm(this.passwordReq, this.user);
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
            console.error(response);
            this.toastr.error(`Failed to save user`);
          }
        }
      });
  }

  deleteUser(): void {
    if (this.currentUser.username === this.user.username) {
      this.toastr.warning(`You cannot delete yourself`);
      return;
    }

    this.userService.delete(this.user.userId)
      .subscribe({
        next: () => {
          this.router.navigate(['/users']);
          this.loading = false;
          this.toastr.success(`Successfully deleted user`);
        },
        error: (response: HttpErrorResponse) => {
          console.error(response);
          this.toastr.error(`Failed to delete user`);
          this.loading = false;}
      });
  }

  resetPassword() {
    const initialState = {
        title: 'Reset Password',
        userId: this.user.userId,
        userName: this.user.username
    };
    this.resetPasswordModalRef = this.modalService.show(ResetPasswordModalComponent, {initialState});
  }

  private setupForm(passReq: PasswordRequirements, data: User) {

    this.userForm = this.formBuilder.group({
      username: [data.username, Validators.required],
      password: [data.password, [Validators.required, Validators.minLength(passReq?.minLength ?? 8)]],
      passwordCompare: [data.passwordCompare, Validators.required],
      firstName: [data.firstName, Validators.required],
      lastName: [data.lastName, Validators.required],
      emailAddress: [data.emailAddress, Validators.email],
      role: [data.role ?? 'Reader', Validators.required],
      lastLogin: [data.lastLogin], 
      failedLoginCount: [data.failedLoginCount ?? 0],
      mustChangePassword: [data.mustChangePassword ?? true],
      enabled: [data.enabled ?? true],
      tagColor: [data.tagColor]
    }, 
    {
      validator: MustMatch('password', 'passwordCompare')
    });

    if (data.userId > 0) {
      this.userForm.get('username').disable();
      this.userForm.get('password').disable();
      this.userForm.get('passwordCompare').disable();
    }
  }

  private formToUser(): User {
    const user = Object.assign({}, this.user);

    user.username = this.f.username.value;
    user.password = this.f.password.value;
    user.passwordCompare = this.f.passwordCompare.value;
    user.firstName = this.f.firstName.value;
    user.lastName = this.f.lastName.value;
    user.emailAddress = this.f.emailAddress.value;
    user.mustChangePassword = this.f.mustChangePassword.value;
    user.enabled = this.f.enabled.value;
    user.role = this.f.role.value;
    user.tagColor = this.f.tagColor.value;
    
    return user;
  }
}
