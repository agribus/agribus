import { TuiRoot } from "@taiga-ui/core";
import { Component, inject } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { TranslateModule } from "@ngx-translate/core";
import { TranslateService } from "@ngx-translate/core";
import { HelloWorldComponent } from "@components/hello-world/hello-world.component";

@Component({
  selector: "app-root",
  imports: [RouterOutlet, TuiRoot, TranslateModule, HelloWorldComponent],
  templateUrl: "./app.component.html",
  styleUrl: "./app.component.scss",
})
export class AppComponent {
  private readonly translateService = inject(TranslateService);
  constructor() {
    this.translateService.addLangs(["fr", "en", "de"]);
    this.translateService.setDefaultLang("fr");
    this.translateService.use("fr");
  }
}
