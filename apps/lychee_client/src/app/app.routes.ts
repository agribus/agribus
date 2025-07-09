import { Routes } from "@angular/router";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HelloWorldComponent } from "@components/hello-world/hello-world.component";
import { mobileOnlyGuard } from "@guards/mobile-only.guard";

export const routes: Routes = [
  { path: "", component: HelloWorldComponent, canActivate: [mobileOnlyGuard] },
  {
    path: "unsupported-platform",
    component: UnsupportedPlatformComponent,
  },
];
