import { inject, Injectable, signal } from "@angular/core";
import { AuthLogin, AuthRegister, AuthResponse } from "@interfaces/auth.interface";
import { HttpClient } from "@angular/common/http";
import { BehaviorSubject, filter, map, Observable, of, shareReplay, take, tap } from "rxjs";
import { environment } from "@environment/environment";

@Injectable({
  providedIn: "root",
})
export class AuthService {
  private http = inject(HttpClient);

  private isLoggedIn$ = new BehaviorSubject<boolean | null>(null);
  private authCheckInProgress = false;

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
          if (response.success) {
            this.isLoggedIn$.next(true);
            this.token.set(response.message);
          }
        })
      );
  }

  public sendRegisterRequest(credentials: AuthRegister): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/users/signup`, credentials);
  }

  public sendLogoutRequest(): Observable<AuthResponse> {
    return this.http
      .post<AuthResponse>(`${environment.apiUrl}/users/logout`, null, {
        headers: {
          Authorization: `Bearer ${this.token()}`,
        },
      })
      .pipe(
        tap(() => {
          this.isLoggedIn$.next(false);
          this.token.set(null);
        })
      );
  }

  public isUserAuthenticated(): Observable<boolean> {
    if (this.isLoggedIn$.value !== null) {
      return of(this.isLoggedIn$.value);
    }

    if (this.authCheckInProgress) {
      return this.isLoggedIn$.pipe(
        filter(value => value !== null),
        take(1)
      );
    }

    this.authCheckInProgress = true;

    return this.http
      .get<{ message: boolean }>(`${environment.apiUrl}/users/me`, {
        headers: {
          Authorization: `Bearer ${this.token()}`,
        },
      })
      .pipe(
        tap(response => {
          this.isLoggedIn$.next(response.message);
          this.authCheckInProgress = false;
        }),
        map(response => response.message),
        shareReplay(1)
      );
  }

  public isLoggedIn(): boolean {
    return !!this.isLoggedIn$.value;
  }
}
