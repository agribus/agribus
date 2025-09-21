import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiTitle } from "@taiga-ui/core";
import { ToggleCardComponent } from "@components/settings/toggle-card/toggle-card.component";
import { GoToCardComponent } from "@components/settings/go-to-card/go-to-card.component";
import { AuthService } from "@services/auth/auth.service";
import { SettingsService } from "@services/settings/settings.service";
import { Router } from "@angular/router";
import { ConfirmComponent } from "@components/settings/confirm/confirm.component";

@Component({
  selector: "app-settings",
  imports: [TuiTitle, ToggleCardComponent, TranslatePipe, GoToCardComponent, ConfirmComponent],
  templateUrl: "./settings.component.html",
  styleUrl: "./settings.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SettingsComponent {
  private readonly authService: AuthService = inject(AuthService);
  private readonly router = inject(Router);
  protected readonly settingsService = inject(SettingsService);

  public handleAlertToggle(value: boolean): void {
    if (value) {
      console.log("Alert is enabled");
    } else {
      console.log("Alert is disabled");
    }
  }

  public handleReportToggle(value: boolean): void {
    if (value) {
      console.log("Report is enabled");
    } else {
      console.log("Report is disabled");
    }
  }

  public handleDarkModeToggle(): void {
    this.settingsService.switchDarkMode();
  }

  public handleGreenhouses(): void {
    this.router.navigate(["/settings/greenhouses"]);
  }

  public handlePasswordChange(): void {
    this.router.navigate(["/password-change"]);
  }

  public handleLogout(): void {
    this.authService.sendLogoutRequest();
    this.router.navigate(["/login"]);
  }

  public handleDeleteAccount(): void {
    this.authService.sendDeleteAccountRequest().subscribe({
      next: res => {
        if (res.success) {
          this.router.navigate(["/login"]);
        }
      },
    });
  }
}
