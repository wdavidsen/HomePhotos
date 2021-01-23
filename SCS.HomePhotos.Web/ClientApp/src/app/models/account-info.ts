import { User } from '.';

export class AccountInfo {
    userId: number;
    username: string;
    firstName: string;
    lastName: string;
    lastLogin: Date;
    role: string;
    emailAddress: string;

    static FromUser(user: User): AccountInfo {
        const a = new AccountInfo();

        a.userId = user != null ? user.userId : 0;
        a.username = user != null ? user.username : '';
        a.firstName = user != null ? user.firstName : '';
        a.lastName = user != null ? user.lastName : '';
        a.emailAddress = user != null ? user.emailAddress : '';
        a.lastLogin = user != null ? user.lastLogin : new Date();
        a.role = user != null ? user.role : '';

        return a;
    }
}
