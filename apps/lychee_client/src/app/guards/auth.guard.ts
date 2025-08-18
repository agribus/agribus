import { CanActivateFn, CanMatchFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { AuthService } from "@services/auth/auth.service";
import { map } from "rxjs";

export const authGuard: CanActivateFn & CanMatchFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.isUserAuthenticated().pipe(
    map(isAuth => {
      return isAuth ? true : router.createUrlTree(["/login"]);
    })
  );
};
