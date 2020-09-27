export class UserRow {
    userId: number;
    username: string;
    firstName: string;
    lastName: string;
    lastLogin: Date;
    failedLoginCount: Number;
    enabled: Boolean;
    role: string;

    selected: boolean;
}
