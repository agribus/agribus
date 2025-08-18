import { TUI_DARK_MODE, TuiRoot } from "@taiga-ui/core";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { DevToolsComponent } from "@components/dev/dev-tools/dev-tools.component";
import { AuthService } from "@services/auth/auth.service";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, TuiRoot, TranslateModule, DevToolsComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);
  private readonly authService = inject(AuthService);
  protected readonly darkMode = inject(TUI_DARK_MODE);

  private readonly lang = localStorage.getItem("lang");
  public authLoaded = true;

  constructor() {
    this.authService.isUserAuthenticated().subscribe({
      next: () => {
        this.authLoaded = true;
      },
      error: () => {
        this.authLoaded = true;
      },
    });
    this.translateService.addLangs(["fr", "en", "de"]);
    const currentLang = this.lang || "fr";
    this.translateService.setDefaultLang(currentLang);
    this.translateService.use(currentLang);
  }
}
