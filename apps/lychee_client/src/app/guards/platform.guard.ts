import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { PlatformService } from "@services/platform/platform.service";

export const platformGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const platformService = inject(PlatformService);

  if (platformService.isDesktop() && state.url !== "/unsupported-platform") {
    router.navigate(["/unsupported-platform"]);
    return false;
  }

  if (!platformService.isDesktop() && state.url === "/unsupported-platform") {
    router.navigate(["/home"]);
    return false;
  }

  return true;
};
