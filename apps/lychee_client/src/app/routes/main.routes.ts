import { MainLayoutComponent } from "@components/layouts/main-layout/main-layout.component";
import { authGuard } from "@guards/auth.guard";
import { platformGuard } from "@guards/platform.guard";
import { DashboardComponent } from "@components/dashboard/dashboard.component";
import { ImmersiveLayoutComponent } from "@components/layouts/immersive-layout/immersive-layout.component";
import { HomeComponent } from "@pages/home/home.component";

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
