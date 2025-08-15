import { HttpErrorResponse, HttpInterceptorFn } from "@angular/common/http";
import { inject } from "@angular/core";
import { catchError, throwError } from "rxjs";
import { ApiResponseService } from "@services/api/api-response.service";

export const httpErrorInterceptor: HttpInterceptorFn = (req, next) => {
  const apiResponseService = inject(ApiResponseService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      apiResponseService.handleError(error);
      return throwError(() => error);
    })
  );
};
