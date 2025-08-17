import { inject, Injectable } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
import { TuiAlertService } from "@taiga-ui/core";
import { TranslateService } from "@ngx-translate/core";

@Injectable({
  providedIn: "root",
})
export class ApiResponseService {
  private readonly alerts = inject(TuiAlertService);
  private readonly translate = inject(TranslateService);

  private buildFullMessage(message: string, apiMessage?: string): string {
    return apiMessage ? `${message} (${apiMessage})` : message;
  }

  public handleError(error: HttpErrorResponse): void {
    const status = error.status;
    const apiError = error.error;

    const message =
      this.translate.instant(`shared.errors.http.${status}`) ||
      this.translate.instant("shared.errors.generic.unknown");
    const apiMessage = apiError?.message || apiError;

    const fullMessage = this.buildFullMessage(message, apiMessage);

    this.alerts
      .open(fullMessage, {
        label: this.translate.instant("shared.alerts.errorApi"),
        appearance: "error",
      })
      .subscribe();
  }
}
