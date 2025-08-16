import { Routes } from "@angular/router";
import { platformGuard } from "@guards/platform.guard";
import { UnsupportedPlatformComponent } from "@pages/unsupported-platform/unsupported-platform.component";
import { HomeComponent } from "@components/home/home.component";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { SettingsAccountComponent } from "@pages/settings-account/settings-account.component";
import { SettingsComponent } from "@pages/settings/settings.component";
import { LoginComponent } from "@pages/auth/login/login.component";
import { RegisterComponent } from "@pages/auth/register/register.component";
import { ForgotPasswordComponent } from "@pages/auth/forgot-password/forgot-password.component";
import { HeaderType } from "@enums/header-type";
import { GreenhouseSettingsComponent } from "@pages/greenhouses/greenhouse-settings/greenhouse-settings.component";
import { GreenhouseCreateComponent } from "@pages/greenhouses/greenhouse-create/greenhouse-create.component";
import { authGuard } from "@guards/auth.guard";
import { guestGuard } from "@guards/guest.guard";
import { NotFoundComponent } from "@pages/not-found/not-found.component";

export const routes: Routes = [
  {
    path: "unsupported-platform",
    component: UnsupportedPlatformComponent,
    canActivate: [platformGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "login",
    component: LoginComponent,
    canActivate: [guestGuard, platformGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "register",
    component: RegisterComponent,
    canActivate: [platformGuard, guestGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "forgot-password",
    component: ForgotPasswordComponent,
    canActivate: [platformGuard, guestGuard],
    data: { headerType: HeaderType.None },
  },
  {
    path: "",
    component: HomeComponent,
    canActivate: [authGuard, platformGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "home",
    component: HomeComponent,
    canActivate: [authGuard, platformGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "dashboard",
    component: DashboardComponent,
    canActivate: [platformGuard, authGuard],
    data: { headerType: HeaderType.Default },
  },
  {
    path: "greenhouse/create",
    component: GreenhouseCreateComponent,
    canActivate: [platformGuard, authGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "greenhouse/settings/:id",
    component: GreenhouseSettingsComponent,
    canActivate: [platformGuard, authGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "settings",
    component: SettingsComponent,
    canActivate: [platformGuard, authGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "settings-account",
    component: SettingsAccountComponent,
    canActivate: [platformGuard, authGuard],
    data: { headerType: HeaderType.Settings },
  },
  {
    path: "**",
    component: NotFoundComponent,
    canActivate: [platformGuard, guestGuard],
    data: {
      headerType: HeaderType.None,
    },
  },
];
