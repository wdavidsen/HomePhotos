import { User } from '.';

export class AccountInfo {    
    userId?: number;
    username?: string;
    firstName?: string;
    lastName?: string;
    lastLogin?: Date;
    role?: string;
    emailAddress?: string;
    avatarImage?: string;
    tagColor?: string;

    static FromUser(user: User): AccountInfo {
        const a = new AccountInfo();

        a.userId = user.userId;
        a.username = user.username;
        a.firstName = user.firstName;
        a.lastName = user.lastName;
        a.emailAddress = user.emailAddress;
        a.avatarImage = user.avatarImage;
        a.lastLogin = user.lastLogin;
        a.role = user.role;
        a.tagColor = user.tagColor;

        return a;
    }
}
