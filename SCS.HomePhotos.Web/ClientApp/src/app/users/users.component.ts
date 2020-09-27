import { Component, OnInit } from '@angular/core';
import { UserService, AuthenticationService } from '../services';
import { UserRow, User } from '../models';
import { map } from 'rxjs/operators';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-users',
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
    private _users: User[];
    private currentUser: User;
    public users: UserRow[];
    public loading = false;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private toastr: ToastrService) {

      this.authenticationService.currentUser.subscribe(user => {
        this.currentUser = user;
      });
    }

  ngOnInit() {
    this.userService.getAll()
        .pipe(map(users => this.usersToUserRows(users)))
        .subscribe((userRows => this.users = userRows));
  }

  select(user): void {
      user.selected = !user.selected;
  }

  addNewUser(): void {
    this.router.navigate(['/users', 0]);
  }

  editUser(user): void {
    this.router.navigate(['/users', user.userId]);
  }

  deleteUser(): void {
    if (confirm('Are you sure you want to delete ALL selected users?')) {
        this.getUpdatableUsers('delete').forEach(userRow => this.userService.delete(userRow.userId)
            .subscribe(
                () => {
                    const users = this._users.filter(u => u.userId !== userRow.userId);
                    this.users = this.usersToUserRows(users);
                    const msg = `Successfully deleted ${userRow.username}`;
                    this.toastr.success(msg);
                },
                error => {
                    console.error(error);
                    const msg = `Failed to delete ${userRow.username}`;
                    this.toastr.error(msg);
                }));
    }
  }

  enableDisableUser(enabled: boolean): void {
    const action = enabled ? 'enable' : 'disable';

    this.getUpdatableUsers(action).forEach(userRow => {
        const user = this._users.find(u => u.userId === userRow.userId);
        user.enabled = enabled;
        this.userService.save(user)
            .subscribe(
                savedUser => {
                    userRow.enabled = user.enabled = savedUser.enabled;
                    const msg = `Successfully ${action} ${user.username}`;
                    this.toastr.success(msg);
                },
                error => {
                    console.error(error);
                    const msg = `Failed to ${action } ${user.username}`;
                    this.toastr.error(msg);
                });
    });
  }

  getSelectedUsers(): UserRow[] {
    return this.users ? this.users.filter(user => user.selected) : [];
  }

  private getUpdatableUsers(action: string): UserRow[] {
    let users = this.users ? this.users.filter(user => user.selected) : [];

    if (users.find(u => u.username === this.currentUser.username)) {
      this.toastr.warning(`Cannot ${action} yourself`);
      users = users.filter(u => u.username !== this.currentUser.username);
    }
    return users;
  }

  private usersToUserRows(users: User[]): UserRow[] {
    this._users = users;
    const userRows = new Array<UserRow>();
    users.forEach(user => userRows.push(this.userToUserRow(user)));
    return userRows;
  }

  private userToUserRow(user: User): UserRow {
    const userRow = new UserRow();

    userRow.userId = user.userId;
    userRow.username = user.username;
    userRow.firstName = user.firstName;
    userRow.lastName = user.lastName;
    userRow.lastLogin = user.lastLogin;
    userRow.failedLoginCount = user.failedLoginCount;
    userRow.enabled = user.enabled;
    userRow.role = user.role;

    return userRow;
  }
}
