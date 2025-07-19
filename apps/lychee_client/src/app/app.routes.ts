import { Routes } from "@angular/router";
import { mobileOnlyGuard } from "@guards/mobile-only.guard";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HomeComponent } from "@components/home/home.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { SettingsAccountComponent } from "@components/settings-account/settings-account.component";
import { GreenhouseFormComponent } from "@components/greenhouse-form/greenhouse-form.component";
import { SettingsComponent } from "@pages/settings/settings.component";

export const routes: Routes = [
  { path: "", component: HomeComponent, canActivate: [mobileOnlyGuard] },
  { path: "home", component: HomeComponent, canActivate: [mobileOnlyGuard] },
  {
    path: "dashboard",
    component: DashboardComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "greenhouse-form",
    component: GreenhouseFormComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "settings",
    component: SettingsComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "settings-account",
    component: SettingsAccountComponent,
    canActivate: [mobileOnlyGuard],
  },

  {
    path: "unsupported-platform",
    component: UnsupportedPlatformComponent,
  },
];
