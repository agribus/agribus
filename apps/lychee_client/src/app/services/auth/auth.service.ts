import { inject, Injectable, signal } from "@angular/core";
import { AuthLogin, AuthRegister, AuthResponse } from "@interfaces/auth.interface";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, Observable, of, tap } from "rxjs";
import { environment } from "@environment/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);

  private isLoggedIn$ = new BehaviorSubject<boolean>(!!localStorage.getItem("access_token"));
  public token = signal<string | null>(localStorage.getItem("access_token"));

  public sendLoginRequest(credentials: AuthLogin): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/login`, credentials, {
        headers: {
          Authorization: `Bearer ${this.token()}`,
        },
      })
      .pipe(
        tap(response => {
          if (response.success && response.token) {
            localStorage.setItem("access_token", response.token);
            this.token.set(response.token);
            this.isLoggedIn$.next(true);
          }
        })
      );
  }

  public sendRegisterRequest(credentials: AuthRegister): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/signup`, credentials);
  }

  public sendLogoutRequest() {
    localStorage.removeItem("access_token");
    this.token.set(null);
    this.isLoggedIn$.next(false);
  }

  isUserAuthenticated(): Observable<boolean> {
    return of(this.isLoggedIn$.value);
  }

  isLoggedIn(): boolean {
    return this.isLoggedIn$.value;
  }
}
