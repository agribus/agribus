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

export interface ForgotPassword {
  readonly email: string;
}

export interface AuthResponse {
  success: boolean;
  message: string;
  token: string;
  errors: {
    [key: string]: string[] | undefined;
  };
}
