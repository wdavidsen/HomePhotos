import { Component, OnInit } from '@angular/core';
import { AccountInfo } from '../models';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { AccountService, AuthService } from '../services';
import { ToastrService } from 'ngx-toastr';
import { BsModalService, BsModalRef } from 'ngx-bootstrap/modal';
import { ModalContentComponent } from '../users/change-password-modal.component';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
  selector: 'app-account',
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  accountInfo: AccountInfo;
  accountForm: FormGroup;
  loading = false;
  submitted = false;
  changePasswordModalRef: BsModalRef;

  constructor(private router: Router,
    private authenticationService: AuthService,
    private accountService: AccountService,
    private formBuilder: FormBuilder,
    private toastr: ToastrService,
    private modalService: BsModalService) { }

  ngOnInit() {
    this.accountInfo = new AccountInfo();
    this.setupForm(this.accountInfo);

    this.authenticationService.getCurrentUser()
      .pipe(map(u => AccountInfo.FromUser(u)))
      .subscribe(
        data => {
            this.accountInfo = data;
            this.setupForm(data);
            this.loading = false;
        });
  }

  // convenience getter for easy access to form fields
  get f() { return this.accountForm.controls; }

  onSubmit() {
    this.submitted = true;

    if (this.accountForm.invalid) {
        return;
    }

    this.loading = true;
    const tempUser = this.formToUser();

    this.accountService.save(tempUser)
      .subscribe(data => {
        this.toastr.success(`Saved successfully`);
        this.accountInfo = tempUser;
        this.loading = false;
        this.setupForm(data);
      },
      error => {
          console.error(error);
          this.toastr.error(`Failed to save`);
          this.loading = false;
      });
  }

  changePassword() {
    const initialState = {
        title: 'Change Password',
        adminMode: false,
        userName: this.accountInfo.username
    };
    this.changePasswordModalRef = this.modalService.show(ModalContentComponent, {initialState});
  }

  logout() {
    this.authenticationService.logout()
      .subscribe(
        () => {
            this.toastr.success('Sign-out successful');
            this.router.navigate(['/login']);
        },
        error => {
            console.error(error);
            this.toastr.error('Sign-out failed');
        });
  }

  private setupForm(data: AccountInfo) {

    this.accountForm = this.formBuilder.group({
        username: [data ? data.username : '', Validators.required],
        firstName: [data ? data.firstName : '', Validators.required],
        lastName: [data ? data.lastName : '', Validators.required],
      });
  }

  private formToUser(): AccountInfo {
    const accountInfo = Object.assign({}, this.accountInfo);

    accountInfo.username = this.f.username.value;
    accountInfo.firstName = this.f.firstName.value;
    accountInfo.lastName = this.f.lastName.value;

    return accountInfo;
  }
}
