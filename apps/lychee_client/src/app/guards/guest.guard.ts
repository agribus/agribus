import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { AuthService } from "@services/auth/auth.service";
import { map } from "rxjs";

export const guestGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.isUserAuthenticated().pipe(
    map(isAuth => {
      if (isAuth) {
        router.navigate(["/home"]);
        return false;
      }
      return true;
    })
  );
};
