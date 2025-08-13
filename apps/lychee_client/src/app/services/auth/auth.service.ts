import { inject, Injectable, signal } from "@angular/core";
import { AuthLogin, AuthRegister, AuthResponse } from "@interfaces/auth.interface";
import { HttpClient } from "@angular/common/http";
import { map, tap } from "rxjs";
import { environment } from "@environment/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);
  public isLoggedIn = signal<boolean>(false);
  token: string = "";

  SendLoginRequest(credentials: AuthLogin) {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/login`, credentials, { withCredentials: true })
      .pipe(
        tap(response => {
          if (response.success) {
            this.isLoggedIn.set(true);
          }
        })
      );
  }

  SendRegisterRequest(credentials: AuthRegister) {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/signup`, credentials);
  }

  SendLogoutRequest() {
    return this.http.post(`${environment.apiUrl}/users/logout`, null, { withCredentials: true });
  }

  IsUserAuthenticated() {
    return this.http
      .get<{ message: boolean }>(`${environment.apiUrl}/users/me`, { withCredentials: true })
      .pipe(
        tap(response => this.isLoggedIn.set(response.message)),
        map(response => response.message)
      );
  }
}
