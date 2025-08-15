import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { ErrorHandlingService } from "@services/api/error-handling/error-handling.service";
import { catchError, mergeMap, tap, throwError } from "rxjs";

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const errorService = inject(ErrorHandlingService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      return errorService.parseHttpError(error).pipe(
        tap(({ message, apiError }) => {
          errorService.showError(message, apiError);
        }),
        mergeMap(err => throwError(() => err))
      );
    })
  );
};
