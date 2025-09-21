import { Injectable, inject, signal } from "@angular/core";
import { TUI_DARK_MODE } from "@taiga-ui/core";

@Injectable({
  providedIn: "root",
})
export class SettingsService {
  public darkMode = inject(TUI_DARK_MODE);
  public darkModeStorage: string | null = localStorage.getItem("darkMode");
  public isDarkModeToggled = signal(this.darkModeStorage === "true");

  public switchDarkMode() {
    this.darkMode.set(!this.darkMode());
    localStorage.setItem("darkMode", this.darkMode().toString());
  }
}
