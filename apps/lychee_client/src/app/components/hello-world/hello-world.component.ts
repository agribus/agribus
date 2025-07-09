import { Component, inject, signal } from "@angular/core";
import { TuiBadge, TuiStatus } from "@taiga-ui/kit";
import { PlatformService } from "@services/platform/platform.service";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TuiButton } from "@taiga-ui/core";

@Component({
  selector: "app-hello-world",
  imports: [TuiBadge, TuiStatus, TranslatePipe, TuiButton],
  templateUrl: "./hello-world.component.html",
  styleUrl: "./hello-world.component.scss",
})
export class HelloWorldComponent {
  public message = signal<string>("");

  private readonly platformService = inject(PlatformService);
  private readonly translateService = inject(TranslateService);

  constructor() {
    this.message.set(this.platformService.isBrowser() ? "hello.browser" : "hello.mobile");
  }

  useLanguage(language: string): void {
    this.translateService.use(language);
  }
}
