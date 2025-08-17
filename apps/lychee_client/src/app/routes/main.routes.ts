import { MainLayoutComponent } from "@components/layouts/main-layout/main-layout.component";
import { HomeComponent } from "@components/home/home.component";
import { authGuard } from "@guards/auth.guard";
import { platformGuard } from "@guards/platform.guard";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { ImmersiveLayoutComponent } from "@components/layouts/immersive-layout/immersive-layout.component";

export const MAIN_ROUTES = [
  {
    path: "",
    component: ImmersiveLayoutComponent,
    children: [
      {
        path: "home",
        component: HomeComponent,
        canActivate: [authGuard, platformGuard],
      },
    ],
  },
  {
    path: "",
    component: MainLayoutComponent,
    children: [
      {
        path: "dashboard",
        component: DashboardComponent,
        canActivate: [platformGuard, authGuard],
      },
    ],
  },
];
