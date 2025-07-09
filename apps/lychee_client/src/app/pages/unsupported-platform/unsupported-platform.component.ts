import { Component, inject, OnDestroy, signal } from "@angular/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { Subscription } from "rxjs";
import { NgOptimizedImage } from "@angular/common";

@Component({
  selector: "app-unsupported-platform",
  imports: [TranslatePipe, NgOptimizedImage],
  templateUrl: "./unsupported-platform.component.html",
  styleUrl: "./unsupported-platform.component.scss",
})
export class UnsupportedPlatformComponent implements OnDestroy {
  private translateService = inject(TranslateService);
  private langChangeSub: Subscription;

  public imageLang = signal<string>("");

  constructor() {
    this.setImageLang(this.translateService.currentLang);

    this.langChangeSub = this.translateService.onLangChange.subscribe(event => {
      this.setImageLang(event.lang);
    });
  }

  private setImageLang(lang: string): void {
    const mainLang = lang.split("-")[0].toUpperCase();
    this.imageLang.set(mainLang);
  }

  ngOnDestroy() {
    this.langChangeSub?.unsubscribe();
  }
}
