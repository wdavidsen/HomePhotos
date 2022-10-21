export class User {
  userId?: number;
  username?: string;
  password?: string;
  passwordCompare?: string;
  firstName?: string;
  lastName?: string;
  emailAddress?: string;
  avatarImage?: string;
  role?: string;
  jwt?: string;
  refreshToken?: string;
  lastLogin?: Date;
  failedLoginCount?: Number;
  mustChangePassword?: Boolean;
  enabled?: Boolean;
}
