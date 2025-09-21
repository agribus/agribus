import { Component, DestroyRef, inject, signal } from "@angular/core";
import { NgOptimizedImage } from "@angular/common";
import { TranslateService, TranslatePipe } from "@ngx-translate/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-unsupported-platform",
  standalone: true,
  imports: [TranslatePipe, NgOptimizedImage],
  templateUrl: "./unsupported-platform.component.html",
  styleUrl: "./unsupported-platform.component.scss",
})
export class UnsupportedPlatformComponent {
  private readonly translateService = inject(TranslateService);
  private readonly destroyRef = inject(DestroyRef);

  /** Signal exposed for the image language (Google Play / App Store badges) */
  public readonly imageLang = signal(this.formatLang(this.translateService.currentLang));

  constructor() {
    // Subscribe to language changes and update imageLang signal accordingly
    this.translateService.onLangChange
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(event => {
        this.imageLang.set(this.formatLang(event.lang));
      });
  }

  private formatLang(lang: string): string {
    return lang.split("-")[0].toUpperCase();
  }
}
