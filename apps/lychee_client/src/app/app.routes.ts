import { Routes } from "@angular/router";
import { mobileOnlyGuard } from "@guards/mobile-only.guard";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HomeComponent } from "@components/home/home.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { SettingsAccountComponent } from "@components/settings-account/settings-account.component";
import { GreenhouseFormOldComponent } from "@components/greenhouse-form-old/greenhouse-form-old.component";
import { SettingsComponent } from "@pages/settings/settings.component";
import { GreenhouseSettingsComponent } from "@pages/greenhouse-settings/greenhouse-settings.component";

export const routes: Routes = [
  { path: "", component: HomeComponent, canActivate: [mobileOnlyGuard] },
  { path: "home", component: HomeComponent, canActivate: [mobileOnlyGuard] },
  {
    path: "dashboard",
    component: DashboardComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "greenhouse-settings",
    component: GreenhouseSettingsComponent,
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
