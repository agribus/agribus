import { MinimalLayoutComponent } from "@components/layouts/minimal-layout/minimal-layout.component";
import { LoginComponent } from "@pages/auth/login/login.component";
import { guestGuard } from "@guards/guest.guard";
import { platformGuard } from "@guards/platform.guard";
import { RegisterComponent } from "@pages/auth/register/register.component";
import { ForgotPasswordComponent } from "@pages/auth/forgot-password/forgot-password.component";

export const AUTH_ROUTES = [
  {
    path: "",
    component: MinimalLayoutComponent,
    children: [
      { path: "login", component: LoginComponent, canActivate: [guestGuard, platformGuard] },
      { path: "register", component: RegisterComponent, canActivate: [guestGuard, platformGuard] },
      {
        path: "forgot-password",
        component: ForgotPasswordComponent,
        canActivate: [guestGuard, platformGuard],
      },
    ],
  },
];
