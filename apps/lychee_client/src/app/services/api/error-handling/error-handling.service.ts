import { inject, Injectable } from "@angular/core";
import { TuiAlertService } from "@taiga-ui/core";
import { TranslateService } from "@ngx-translate/core";
import { HttpErrorResponse } from "@angular/common/http";

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
    let message: string;
    const status = httpError.status;
    const apiError = httpError.error;

    switch (status) {
      case 400:
        message = this.translateService.instant("shared.errors.http.400");
        break;
      case 401:
        message = this.translateService.instant("shared.errors.http.401");
        break;
      case 403:
        message = this.translateService.instant("shared.errors.http.403");
        break;
      case 404:
        message = this.translateService.instant("shared.errors.http.404");
        break;
      case 409:
        message = this.translateService.instant("shared.errors.http.409");
        break;
      case 422:
        message = this.translateService.instant("shared.errors.http.422");
        break;
      case 500:
        message = this.translateService.instant("shared.errors.http.500");
        break;
      case 0:
        message = this.translateService.instant("shared.errors.http.0");
        break;
      default:
        message = this.translateService.instant("shared.errors.http.default");
        break;
    }

    return { message, status, apiError };
  }
}
