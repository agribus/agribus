import { inject } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

export const i18nExtractHelper = () => {
  const translateService = inject(TranslateService);

  /* i18n-extract:start */
  translateService.instant("shared.errors.http.0");
  translateService.instant("shared.errors.http.400");
  translateService.instant("shared.errors.http.401");
  translateService.instant("shared.errors.http.403");
  translateService.instant("shared.errors.http.404");
  translateService.instant("shared.errors.http.409");
  translateService.instant("shared.errors.http.422");
  translateService.instant("shared.errors.http.500");
  /* i18n-extract:end */
};
