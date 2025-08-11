import { Routes } from "@angular/router";
import { mobileOnlyGuard } from "@guards/mobile-only.guard";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HomeComponent } from "@components/home/home.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { SettingsAccountComponent } from "@pages/settings-account/settings-account.component";
import { SettingsComponent } from "@pages/settings/settings.component";
import { LoginComponent } from "@pages/login/login.component";
import { RegisterComponent } from "@pages/register/register.component";
import { ForgotPasswordComponent } from "@pages/forgot-password/forgot-password.component";
import { HeaderType } from "@enums/header-type";
import { GreenhouseSettingsComponent } from "@pages/greenhouse-settings/greenhouse-settings.component";

export const routes: Routes = [
  {
    path: "",
    component: HomeComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "home",
    component: HomeComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "dashboard",
    component: DashboardComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "greenhouse-settings",
    component: GreenhouseSettingsComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "settings",
    component: SettingsComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "settings-account",
    component: SettingsAccountComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.Settings },
  },

  {
    path: "unsupported-platform",
    component: UnsupportedPlatformComponent,
    data: { headerType: HeaderType.None },
  },
  {
    path: "login",
    component: LoginComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "register",
    component: RegisterComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "forgot-password",
    component: ForgotPasswordComponent,
    canActivate: [mobileOnlyGuard],
    data: { headerType: HeaderType.None },
  },
];
