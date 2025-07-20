import { AsyncPipe } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { TranslateService, TranslatePipe } from "@ngx-translate/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

import { TuiButton, TuiError, TuiTextfield } from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { TuiCardLarge } from "@taiga-ui/layout";
import { AuthLogin } from "@interfaces/auth.interface";

@Component({
  selector: "app-login-form",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
    TuiCardLarge,
    TuiError,
    TuiFieldErrorPipe,
    TuiTextfield,
    TranslatePipe,
  ],
  templateUrl: "./login-form.component.html",
  styleUrl: "./login-form.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis",
      email: "Veuillez entrez une adresse mail valide",
    }),
  ],
})
export class LoginFormComponent {
  private readonly translateService = inject(TranslateService);
  protected readonly form = new FormGroup({
    email: new FormControl("", [Validators.required, Validators.email]),
    password: new FormControl("", Validators.required),
  });
  private readonly lang = localStorage.getItem("lang") || "fr";

  constructor() {
    this.translateService.use(this.lang);
  }

  protected onSubmit(event: MouseEvent) {
    event.preventDefault();
    if (this.form.valid) {
      const loginInformation: AuthLogin = {
        email: this.form.value.email ?? "",
        password: this.form.value.password ?? "",
      };
      console.log(loginInformation);
    }
  }
}
