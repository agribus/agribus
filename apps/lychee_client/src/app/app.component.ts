import { TuiRoot } from "@taiga-ui/core";
import { Component, effect, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { NavBarComponent } from "@components/nav-bar/nav-bar.component";
import { HeaderComponent } from "@components/header/header.component";
import { HeaderType } from "@enums/header-type";
import { HeaderStateService } from "@services/header-state.service";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, TuiRoot, TranslateModule, NavBarComponent, HeaderComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);
  private readonly headerStateService = inject(HeaderStateService);

  private readonly lang = localStorage.getItem("lang");
  public showNavbar = true;

  constructor() {
    this.translateService.addLangs(["fr", "en", "de"]);
    if (!this.lang) {
      this.lang = "fr";
    }
    this.translateService.setDefaultLang(this.lang);
    this.translateService.use(this.lang);

    effect(() => {
      const headerType = this.headerStateService.headerType();
      this.showNavbar = headerType !== HeaderType.Settings;
    });
  }
}
