import { Routes } from "@angular/router";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";

export const routes: Routes = [
  { path: "", redirectTo: "unsupported-platform", pathMatch: "full" },
  { path: "unsupported-platform", component: UnsupportedPlatformComponent },
];
