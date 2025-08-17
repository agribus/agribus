import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { AsyncPipe } from "@angular/common";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TuiButton, TuiError, TuiTextfield } from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { ForgotPassword } from "@interfaces/auth.interface";

@Component({
  selector: "app-forgot-password-form",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
    TuiError,
    TuiFieldErrorPipe,
    TuiTextfield,
    TranslatePipe,
  ],
  templateUrl: "./forgot-password-form.component.html",
  styleUrl: "./forgot-password-form.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis",
      email: "Veuillez entrez une adresse mail valide",
    }),
  ],
})
export class ForgotPasswordFormComponent {
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
