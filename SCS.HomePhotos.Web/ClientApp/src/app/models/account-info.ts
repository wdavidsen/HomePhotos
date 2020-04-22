import { User } from '.';

export class AccountInfo {
    userId: number;
    username: string;
    firstName: string;
    lastName: string;
    lastLogin: Date;
    admin: Boolean;

    static FromUser(user: User): AccountInfo {
        const a = new AccountInfo();

        a.userId = user.userId;
        a.username = user.username;
        a.firstName = user.firstName;
        a.lastName = user.lastName;
        a.lastLogin = user.lastLogin;
        a.admin = user.admin;

        return a;
    }
}
