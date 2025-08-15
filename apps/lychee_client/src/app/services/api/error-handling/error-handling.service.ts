import { inject, Injectable } from "@angular/core";
import { TuiAlertService } from "@taiga-ui/core";
import { TranslateService } from "@ngx-translate/core";
import { HttpErrorResponse } from "@angular/common/http";
import { map } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class ErrorHandlingService {
  private readonly alerts = inject(TuiAlertService);
  private readonly translateService = inject(TranslateService);

  public showError(message: string, messageApi?: string) {
    const fullMessage = messageApi ? `${message} (${messageApi})` : message;
    this.alerts
      .open(fullMessage, {
        label: this.translateService.instant("components.ui.alert.error"),
        appearance: "error",
      })
      .subscribe();
  }

  public showSuccess(message: string) {
    this.alerts
      .open(message, {
        label: this.translateService.instant("components.ui.alert.success"),
        appearance: "positive",
      })
      .subscribe();
  }

  public parseHttpError(httpError: HttpErrorResponse) {
    const status = httpError.status;
    const apiError = httpError.error;
    const translationKey = `shared.errors.http.${status}`;

    return this.translateService.get(translationKey).pipe(
      map(message => ({
        message: message || this.translateService.instant("shared.errors.http.default"),
        status,
        apiError: apiError?.message || apiError,
      }))
    );
  }
}
