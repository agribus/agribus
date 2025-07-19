export interface AuthLogin {
  readonly email: string;
  readonly password: string;
}

export interface AuthRegister {
  readonly username: string;
  readonly email: string;
  readonly password: string;
  readonly confirmPassword: string;
}
