export class User {
  constructor() {
    this.userId = null;
    this.role = 'Reader';
    this.username = null;
    this.firstName = null;
    this.lastName = null;
    this.emailAddress = null;
    this.jwt = null;
    this.refreshToken = null;
    this.lastLogin = null;
    this.failedLoginCount = 0;
    this.mustChangePassword = true;
    this.enabled = true;
  }
  userId: number | null;
  username: string;
  password: string;
  passwordCompare: string;
  firstName: string;
  lastName: string;
  emailAddress: string;
  role: string;
  jwt: string;
  refreshToken: string;
  lastLogin: Date;
  failedLoginCount: Number;
  mustChangePassword: Boolean;
  enabled: Boolean;
}
