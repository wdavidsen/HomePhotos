export class User {
    userId: number;
    username: string;
    password: string;
    passwordCompare: string;
    firstName: string;
    lastName: string;
    token: string;
    refreshToken: string;
    lastLogin: Date;
    failedLoginCount: Number;
    mustChangePassword: Boolean;
    enabled: Boolean;
    admin: Boolean;
}
