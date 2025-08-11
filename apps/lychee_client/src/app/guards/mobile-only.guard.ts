import { CanActivateFn, Router } from "@angular/router";
import { inject } from "@angular/core";
import { PlatformService } from "@services/platform/platform.service";

export const mobileOnlyGuard: CanActivateFn = () => {
  const router = inject(Router);
  const platformService = inject(PlatformService);

  const isDesktop = platformService.isDesktop();

  if (isDesktop) {
    router.navigate(["/unsupported-platform"]);
    return false;
  }

  return true;
};
