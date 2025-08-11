import { TuiRoot } from "@taiga-ui/core";
import { Component, effect, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule, TranslateService } from "@ngx-translate/core";
import { NavbarComponent } from "@components/ui/navbar/navbar.component";
import { HeaderComponent } from "@components/ui/header/header.component";
import { HeaderType } from "@enums/header-type";
import { HeaderStateService } from "@services/header-state.service";
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
      this.showNavbar = headerType !== (HeaderType.Settings || HeaderType.None);
    });
  }
}
