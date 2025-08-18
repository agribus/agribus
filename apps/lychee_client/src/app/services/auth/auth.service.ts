import { inject, Injectable } from "@angular/core";
import { AuthLogin, AuthRegister, AuthResponse } from "@interfaces/auth.interface";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, map, Observable, of, tap } from "rxjs";
import { environment } from "@environment/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);

  private isLoggedIn$ = new BehaviorSubject<boolean | null>(null);

  public sendLoginRequest(credentials: AuthLogin): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/login`, credentials, {
        withCredentials: true,
      })
      .pipe(
        tap(response => {
          if (response.success) {
            this.isLoggedIn$.next(true);
          }
        })
      );
  }

  public sendRegisterRequest(credentials: AuthRegister): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/signup`, credentials);
  }

  public sendLogoutRequest(): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/logout`, null, { withCredentials: true })
      .pipe(
        tap(() => {
          this.isLoggedIn$.next(false);
        })
      );
  }

  public isUserAuthenticated(): Observable<boolean> {
    if (this.isLoggedIn$.value !== null) {
      return of(this.isLoggedIn$.value);
    }

    return this.http
      .get<{ message: boolean }>(`${environment.apiUrl}/users/me`, { withCredentials: true })
      .pipe(
        tap(response => this.isLoggedIn$.next(response.message)),
        map(response => response.message)
      );
  }

  public isLoggedIn(): boolean {
    return !!this.isLoggedIn$.value;
  }
}
