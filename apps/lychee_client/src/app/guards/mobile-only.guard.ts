import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { Platform } from '@angular/cdk/platform';

export const mobileOnlyGuard: CanActivateFn = () => {
  const router = inject(Router);
  const platform = inject(Platform);

  console.log(platform)
  const isMobile = platform.ANDROID || platform.IOS;

  if (!isMobile) {
    router.navigate(['/unsupported-platform']);
    return false;
  }

  return true;
};
