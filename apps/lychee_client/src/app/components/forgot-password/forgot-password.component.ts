import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { AsyncPipe, NgOptimizedImage } from "@angular/common";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TuiButton, TuiError, TuiTextfield, TuiTitle } from "@taiga-ui/core";
import { TuiCardLarge, TuiHeader } from "@taiga-ui/layout";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { ForgotPassword } from "@interfaces/auth.interface";

@Component({
  selector: "app-forgot-password",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
    TuiCardLarge,
    TuiError,
    TuiFieldErrorPipe,
    TuiHeader,
    TuiTextfield,
    TuiTitle,
    NgOptimizedImage,
    TranslatePipe,
  ],
  templateUrl: "./forgot-password.component.html",
  styleUrl: "./forgot-password.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis",
      email: "Veuillez entrez une adresse mail valide",
    }),
  ],
})
export class ForgotPasswordComponent {
  private readonly translateService = inject(TranslateService);
  protected readonly form = new FormGroup({
    email: new FormControl("", [Validators.required, Validators.email]),
  });
  private readonly lang = localStorage.getItem("lang") || "fr";

  constructor() {
    this.translateService.use(this.lang);
  }

  protected onSubmit(event: MouseEvent) {
    event.preventDefault();
    if (this.form.valid) {
      const emailInformation: ForgotPassword = {
        email: this.form.value.email ?? "",
      };
      console.log(emailInformation);
    }
  }
}
