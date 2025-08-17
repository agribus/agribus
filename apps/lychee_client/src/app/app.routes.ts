import { Routes } from "@angular/router";
import { MAIN_ROUTES } from "./routes/main.routes";
import { SETTINGS_ROUTES } from "./routes/settings.routes";
import { AUTH_ROUTES } from "./routes/auth.routes";
import { ERROR_ROUTES } from "./routes/error.routes";

export const routes: Routes = [
  {
    path: "",
    redirectTo: "home",
    pathMatch: "full",
  },
  ...MAIN_ROUTES,
  ...SETTINGS_ROUTES,
  ...AUTH_ROUTES,
  ...ERROR_ROUTES,
];
