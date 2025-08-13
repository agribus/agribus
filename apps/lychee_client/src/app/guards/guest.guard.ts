import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { AuthService } from "@services/auth/auth.service";
import { map, tap } from "rxjs";

export const guestGuard: CanActivateFn = () => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.IsUserAuthenticated().pipe(
    tap(isAuth => console.log("Authenticated:", isAuth)),
    map(isAuth => {
      if (isAuth) {
        router.navigate(["/home"]);
        return false;
      }
      return true;
    })
  );
};
