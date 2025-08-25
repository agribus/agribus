import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { platformGuard } from "@guards/platform.guard";
import { NotFoundComponent } from "@pages/not-found/not-found.component";
import { guestGuard } from "@guards/guest.guard";

export const ERROR_ROUTES = [
  {
    path: "unsupported-platform",
    component: UnsupportedPlatformComponent,
    canActivate: [platformGuard],
  },
  {
    path: "**",
    component: NotFoundComponent,
    canActivate: [platformGuard, guestGuard],
  },
];
