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
import { GreenhouseSettingsComponent } from "@pages/greenhouses/greenhouse-settings/greenhouse-settings.component";
import { GreenhouseCreateComponent } from "@pages/greenhouses/greenhouse-create/greenhouse-create.component";
import { authGuard } from "@guards/auth.guard";
import { guestGuard } from "@guards/guest.guard";
import { NotFoundComponent } from "@pages/not-found/not-found.component";
import { AuthLayoutComponent } from "@components/layouts/auth-layout/auth-layout.component";
import { MainLayoutComponent } from "@components/layouts/main-layout/main-layout.component";
import { SettingsLayoutComponent } from "@components/layouts/settings-layout/settings-layout.component";

export const routes: Routes = [
  {
    path: "",
    redirectTo: "home",
    pathMatch: "full",
  },
  {
    path: "",
    component: MainLayoutComponent,
    children: [
      {
        path: "home",
        component: HomeComponent,
        canActivate: [authGuard, platformGuard],
      },
      {
        path: "dashboard",
        component: DashboardComponent,
        canActivate: [platformGuard, authGuard],
      },
    ],
  },
  {
    path: "",
    component: SettingsLayoutComponent,
    children: [
      {
        path: "greenhouse/create",
        component: GreenhouseCreateComponent,
        canActivate: [platformGuard, authGuard],
      },
      {
        path: "greenhouse/settings/:id",
        component: GreenhouseSettingsComponent,
        canActivate: [platformGuard, authGuard],
      },
      {
        path: "settings",
        component: SettingsComponent,
        canActivate: [platformGuard, authGuard],
      },
      {
        path: "settings-account",
        component: SettingsAccountComponent,
        canActivate: [platformGuard, authGuard],
      },
    ],
  },
  {
    path: "",
    component: AuthLayoutComponent,
    children: [
      { path: "login", component: LoginComponent, canActivate: [guestGuard, platformGuard] },
      { path: "register", component: RegisterComponent, canActivate: [guestGuard, platformGuard] },
      {
        path: "forgot-password",
        component: ForgotPasswordComponent,
        canActivate: [guestGuard, platformGuard],
      },
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
    ],
  },
];
