import { Routes } from "@angular/router";
import { mobileOnlyGuard } from "@guards/mobile-only.guard";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HomeComponent } from "@components/home/home.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { SettingsAccountComponent } from "@components/settings-account/settings-account.component";
import { SettingsComponent } from "@pages/settings/settings.component";
import { LoginComponent } from "@pages/login/login.component";
import { RegisterComponent } from "@pages/register/register.component";
import { ForgotPasswordComponent } from "@pages/forgot-password/forgot-password.component";
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
  {
    path: "login",
    component: LoginComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "register",
    component: RegisterComponent,
    canActivate: [mobileOnlyGuard],
  },
  {
    path: "forgot-password",
    component: ForgotPasswordComponent,
    canActivate: [mobileOnlyGuard],
  },
];
