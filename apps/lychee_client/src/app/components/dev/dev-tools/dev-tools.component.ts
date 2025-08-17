import { Component, inject, Injectable, signal } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { TuiActionBar } from "@taiga-ui/kit";
import { TUI_DARK_MODE, TuiButton, TuiDialogService } from "@taiga-ui/core";
import { DevToolsService } from "@services/dev-tools/dev-tools.service";
import { TranslateService } from "@ngx-translate/core";
import { PlatformService } from "@services/platform/platform.service";
import { PolymorpheusComponent } from "@taiga-ui/polymorpheus";
import { RouteSelectorDialogComponent } from "@components/dev/route-selector-dialog/route-selector-dialog.component";
import { AuthService } from "@services/auth/auth.service";
import { Router } from "@angular/router";

@Component({
  selector: "app-dev-tools",
  imports: [FormsModule, TuiActionBar, TuiButton, ReactiveFormsModule],
  templateUrl: "./dev-tools.component.html",
  styleUrl: "./dev-tools.component.scss",
})
@Injectable({ providedIn: "root" })
export class DevToolsComponent {
  private devToolsService = inject(DevToolsService);
  private readonly translateService = inject(TranslateService);
  private readonly platformService = inject(PlatformService);
  private readonly dialogService = inject(TuiDialogService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly darkMode = inject(TUI_DARK_MODE);

  logs = signal<string[]>([]);
  isNativePlatform = this.platformService.isNativePlatform();

  constructor() {
    if (this.isNativePlatform) {
      this.interceptConsole();
    }
    console.log(this.platformService.getDeviceInfo());
  }

  get openDevTools() {
    return this.devToolsService.open();
  }

  closeDevTools(value: boolean) {
    this.devToolsService.open.set(value);
  }

  public openRouteDialog() {
    this.dialogService
      .open<void>(new PolymorpheusComponent(RouteSelectorDialogComponent), {
        size: "m",
        closeable: true,
        dismissible: true,
        required: false,
        label: "Select a route",
      })
      .subscribe();
    this.closeDevTools(false);
  }

  useLanguage(language: string): void {
    localStorage.setItem("lang", language);
    this.translateService.use(language);
  }

  private interceptConsole() {
    const originalLog = console.log;
    const originalError = console.error;
    const originalWarn = console.warn;

    console.log = (...args: unknown[]) => {
      this.appendLog("LOG", args);
      originalLog.apply(console, args);
    };

    console.error = (...args: unknown[]) => {
      this.appendLog("ERROR", args);
      originalError.apply(console, args);
    };

    console.warn = (...args: unknown[]) => {
      this.appendLog("WARN", args);
      originalWarn.apply(console, args);
    };
  }

  private appendLog(type: string, args: unknown[]) {
    const formatted = args.map(arg => {
      if (arg instanceof Error) {
        return `${arg.name}: ${arg.message}\n${arg.stack}`;
      }

      if (typeof arg === "object") {
        try {
          return JSON.stringify(arg, null, 2);
        } catch {
          return "[Circular Object]";
        }
      }

      return String(arg);
    });

    const message = `[${type}] ${formatted.join(" ")}`;
    this.logs.update(current => [...current, message]);
  }

  clear() {
    this.logs.set([]);
  }

  Logout() {
    this.authService.sendLogoutRequest().subscribe(() => {
      this.router.navigate(["/login"]);
    });
  }
}
