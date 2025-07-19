import { TuiRoot } from "@taiga-ui/core";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule } from "@ngx-translate/core";
import { TranslateService } from "@ngx-translate/core";
import { NavBarComponent } from "@components/nav-bar/nav-bar.component";
import { Router } from "@angular/router";
import { HeaderComponent } from "@components/header/header.component";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, TuiRoot, TranslateModule, NavBarComponent, HeaderComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);
  public router = inject(Router);
  // List of routes without navbar
  private hiddenNavbarRoutes = [
    "/unsupported-platform",
    "/greenhouse-form",
    "/settings-account",
    "/settings",
    "/login",
    "/register",
  ];
  constructor() {
    this.translateService.addLangs(["fr", "en", "de"]);
    this.translateService.setDefaultLang("fr");
    this.translateService.use("fr");
  }
  get showNavbar(): boolean {
    return !this.hiddenNavbarRoutes.some(route => this.router.url.startsWith(route));
  }
}
