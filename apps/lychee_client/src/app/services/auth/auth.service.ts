import { inject, Injectable, signal } from "@angular/core";
import { AuthLogin, AuthRegister, AuthResponse } from "@interfaces/auth.interface";
import { HttpClient } from "@angular/common/http";
import { environment } from "../../../environments/environment";
import { map, tap } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl: string = environment.apiUrl;
  public isLoggedIn = signal<boolean>(false);
  token: string = "";

  SendLoginRequest(credentials: AuthLogin) {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/users/login`, credentials, { withCredentials: true })
      .pipe(
        tap(response => {
          if (response.success) {
            this.isLoggedIn.set(true);
          }
        })
      );
  }

  SendRegisterRequest(credentials: AuthRegister) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/users/signup`, credentials);
  }

  SendLogoutRequest() {
    return this.http.post(`${this.apiUrl}/users/logout`, null, { withCredentials: true });
  }

  IsUserAuthenticated() {
    return this.http
      .get<{ message: boolean }>(`${this.apiUrl}/users/me`, { withCredentials: true })
      .pipe(
        tap(response => this.isLoggedIn.set(response.message)),
        map(response => response.message)
      );
  }
}
