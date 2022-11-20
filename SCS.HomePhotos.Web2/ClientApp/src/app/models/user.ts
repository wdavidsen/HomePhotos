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
  tagColor?: string;

  static isAdmin(user: User) {
    return user.role != null && user.role.toUpperCase() == 'ADMIN';
  }

  static isReader(user: User) {
    return user.role != null && user.role.toUpperCase() == 'READER';
  }

  static isContributer(user: User) {
    return user.role != null && user.role.toUpperCase() == 'CONTRIBUTOR';
  }
}
