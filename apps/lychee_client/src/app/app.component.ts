import { TuiRoot } from "@taiga-ui/core";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule } from "@ngx-translate/core";
import { TranslateService } from "@ngx-translate/core";
import { NavbarComponent } from "@components/ui/navbar/navbar.component";
import { Router } from "@angular/router";
import { HeaderComponent } from "@components/ui/header/header.component";
import { DevToolsComponent } from "@components/dev/dev-tools/dev-tools.component";

@Component({
  selector: "app-root",
  imports: [
    RouterOutlet,
    TuiRoot,
    TranslateModule,
    NavbarComponent,
    HeaderComponent,
    DevToolsComponent,
  ],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);
  public router = inject(Router);
  // List of routes without navbar
  private hiddenNavbarRoutes = [
    "/unsupported-platform",
    "/greenhouse-settings",
    "/settings-account",
    "/settings",
    "/login",
    "/register",
    "/forgot-password",
  ];
  private readonly lang = localStorage.getItem("lang");
  constructor() {
    this.translateService.addLangs(["fr", "en", "de"]);
    if (!this.lang) {
      this.lang = "fr";
    }
    this.translateService.setDefaultLang(this.lang);
    this.translateService.use(this.lang);
  }
  get showNavbar(): boolean {
    return !this.hiddenNavbarRoutes.some(route => this.router.url.startsWith(route));
  }
}
