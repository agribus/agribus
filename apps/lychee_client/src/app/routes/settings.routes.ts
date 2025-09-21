import { SettingsLayoutComponent } from "@components/layouts/settings-layout/settings-layout.component";
import { GreenhouseCreateComponent } from "@pages/greenhouses/greenhouse-create/greenhouse-create.component";
import { platformGuard } from "@guards/platform.guard";
import { authGuard } from "@guards/auth.guard";
import { GreenhouseSettingsComponent } from "@pages/greenhouses/greenhouse-settings/greenhouse-settings.component";
import { SettingsComponent } from "@pages/settings/settings.component";
import { SettingsAccountComponent } from "@pages/settings-account/settings-account.component";
import { PasswordChangeComponent } from "@pages/password-change/password-change.component";
import { GreenhouseListComponent } from "@pages/greenhouses/greenhouse-list/greenhouse-list.component";

export const SETTINGS_ROUTES = [
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
      {
        path: "password-change",
        component: PasswordChangeComponent,
        canActivate: [platformGuard, authGuard],
      },
      {
        path: "settings/greenhouses",
        component: GreenhouseListComponent,
        canActivate: [platformGuard, authGuard],
      },
    ],
  },
];
