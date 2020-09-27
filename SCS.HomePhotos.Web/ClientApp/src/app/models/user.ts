export class User {
  constructor() {
    this.userId = null;
    this.role = 'Reader';
    this.mustChangePassword = true;
    this.enabled = true;
  }
  userId: number | null;
  username: string;
  password: string;
  passwordCompare: string;
  firstName: string;
  lastName: string;
  role: string;
  token: string;
  refreshToken: string;
  lastLogin: Date;
  failedLoginCount: Number;
  mustChangePassword: Boolean;
  enabled: Boolean;
}
