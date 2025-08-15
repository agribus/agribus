import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { ErrorHandlingService } from "@services/api/error-handling/error-handling.service";
import { catchError, throwError } from "rxjs";

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorHandlingService);

  return next(req).pipe(
    catchError((errorResponse: HttpErrorResponse) => {
      const { message, apiError } = errorService.parseHttpError(errorResponse);

      errorService.showError(message, apiError);

      return throwError(() => ({
        message,
        apiError,
        status: errorResponse.status,
        raw: errorResponse,
      }));
    })
  );
};
