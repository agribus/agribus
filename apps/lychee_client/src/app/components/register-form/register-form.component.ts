import { AsyncPipe, NgOptimizedImage } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TuiButton, TuiError, TuiTextfield, TuiTitle } from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { TuiCardLarge, TuiHeader } from "@taiga-ui/layout";
import { AuthRegister } from "@interfaces/auth.interface";
import { TranslateService, TranslatePipe } from "@ngx-translate/core";

@Component({
  selector: "app-register-form",
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
  templateUrl: "./register-form.component.html",
  styleUrl: "./register-form.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis",
      email: "Veuillez entrez une adresse mail valide",
    }),
  ],
})
export class RegisterFormComponent {
  private readonly translateService = inject(TranslateService);
  protected readonly form = new FormGroup({
    username: new FormControl("", [Validators.required]),
    email: new FormControl("", [Validators.required, Validators.email]),
    password: new FormControl("", Validators.required),
    confirmPassword: new FormControl("", [Validators.required]),
  });

  private readonly lang = localStorage.getItem("lang") || "fr";

  constructor() {
    this.translateService.use(this.lang);
  }

  protected onSubmit(event: MouseEvent) {
    event.preventDefault();
    if (this.form.valid) {
      const registerInformation: AuthRegister = {
        username: this.form.value.username ?? "",
        email: this.form.value.email ?? "",
        password: this.form.value.password ?? "",
        confirmPassword: this.form.value.confirmPassword ?? "",
      };
      console.log(registerInformation);
    }
  }
}
