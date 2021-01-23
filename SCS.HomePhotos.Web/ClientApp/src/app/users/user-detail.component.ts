import { Component, OnInit, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService, AuthService } from '../services';
import { User } from '../models';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ModalContentComponent } from './change-password-modal.component';

@Component({
  selector: 'app-user-detail',
  templateUrl: './user-detail.component.html',
  styleUrls: ['./user-detail.component.css']
})
export class UserDetailComponent implements OnInit {
  private currentUser: User;
  user: User;
  userForm: FormGroup;
  roles = ['Reader', 'Contributer', 'Admin'];
  loading = false;
  submitted = false;
  changePasswordModalRef: BsModalRef;

  constructor(
    private route: ActivatedRoute,
    private authenticationService: AuthService,
    private router: Router,
    private formBuilder: FormBuilder,
    private userService: UserService,
    private toastr: ToastrService,
    private modalService: BsModalService) {
      this.authenticationService.loadCsrfToken().subscribe();
      this.authenticationService.getCurrentUser().subscribe(user => {
        this.currentUser = user;
      });
    }

  // convenience getter for easy access to form fields
  get f() { return this.userForm.controls; }

  ngOnInit() {
    this.user = new User();
    this.setupForm(this.user);

    this.route.paramMap.subscribe(params => {
        const userId = params.get('userId');
        console.log(`Received userId: ${userId}`);

        if (userId !== null && parseInt(userId, 10) > 0) {
            this.loading = true;

            this.userService.get(parseInt(userId, 10))
                .subscribe(
                    data => {
                        this.user = data;
                        this.setupForm(data);
                        this.loading = false;
                    },
                    error => {
                        console.error(error);
                        this.toastr.error(`Failed to find user`);
                        this.loading = false;
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
      .subscribe(user => {
          this.user = user;
          this.toastr.success(`User saved successfully`);
          this.loading = false;
          this.setupForm(this.user);
        },
        error => {
            console.error(error);
            this.toastr.error(`Failed to save user`);
            this.loading = false;
        });
  }

  deleteUser(): void {
    if (this.currentUser.username === this.user.username) {
      this.toastr.warning(`You cannot delete yourself`);
      return;
    }

    this.userService.delete(this.user.userId)
        .subscribe(() => {
            this.router.navigate(['/users']);
            this.loading = false;
            this.toastr.success(`Successfully deleted user`);
        },
        error => {
            console.error(error);
            this.toastr.error(`Failed to delete user`);
            this.loading = false;
        });
  }

  resetPassword() {
    const initialState = {
        title: 'Reset Password',
        adminMode: true,
        userName: this.user.username
    };
    this.changePasswordModalRef = this.modalService.show(ModalContentComponent, {initialState});
  }

  private setupForm(data: User) {

    this.userForm = this.formBuilder.group({
      username: [data.username, Validators.required],
      password: [data.password, Validators.required],
      passwordCompare: [data.passwordCompare, Validators.required],
      firstName: [data.firstName, Validators.required],
      lastName: [data.lastName, Validators.required],
      emailAddress: [data.emailAddress, Validators.email],
      role: [data.role, Validators.required],
      lastLogin: [data.lastLogin],
      failedLoginCount: [data.failedLoginCount],
      mustChangePassword: [data.mustChangePassword],
      enabled: [data.enabled]
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

    return user;
  }
}
