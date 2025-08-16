import { TUI_DARK_MODE, TuiRoot } from "@taiga-ui/core";
import { Component, effect, inject } from "@angular/core";
import { ActivatedRoute, Router, RouterOutlet } from "@angular/router";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { NavbarComponent } from "@components/ui/navbar/navbar.component";
import { HeaderComponent } from "@components/ui/header/header.component";
import { HeaderType } from "@enums/header-type";
import { HeaderStateService } from "@services/ui/header-state/header-state.service";
import { DevToolsComponent } from "@components/dev/dev-tools/dev-tools.component";
import { AuthService } from "@services/auth/auth.service";

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
  private readonly headerStateService = inject(HeaderStateService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);
  protected readonly darkMode = inject(TUI_DARK_MODE);

  private readonly lang = localStorage.getItem("lang");
  public showNavbar = false;
  public authLoaded = false;

  constructor() {
    this.authService.isUserAuthenticated().subscribe(() => {
      this.authLoaded = true;
    });
    this.translateService.addLangs(["fr", "en", "de"]);
    if (!this.lang) {
      this.lang = "fr";
    }
    this.translateService.setDefaultLang(this.lang);
    this.translateService.use(this.lang);

    effect(() => {
      const headerType = this.headerStateService.headerType();
      this.showNavbar = headerType !== HeaderType.Settings && headerType !== HeaderType.None;
    });
  }
}
