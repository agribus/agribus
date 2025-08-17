import { AsyncPipe } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { TuiButton, TuiError, TuiTextfield } from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { AuthRegister } from "@interfaces/auth.interface";
import { AuthService } from "@services/auth/auth.service";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { TuiValidationError } from "@taiga-ui/cdk";

@Component({
  selector: "app-register-form",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
    TuiError,
    TuiFieldErrorPipe,
    TuiTextfield,
    TranslatePipe,
  ],
  templateUrl: "./register-form.component.html",
  styleUrl: "./register-form.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis.",
      email: "Veuillez entrez une adresse mail valide.",
      minlength: "Le mot de passe doit faire au moins 8 caractères.",
      maxlength: "Le mot de passe ne doit pas dépasser 32 caractères.",
    }),
  ],
})
export class RegisterFormComponent {
  private readonly translateService = inject(TranslateService);
  private readonly authService: AuthService = inject(AuthService);
  protected readonly form = new FormGroup({
    username: new FormControl("", [Validators.required]),
    email: new FormControl("", [Validators.required, Validators.email]),
    password: new FormControl("", [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(32),
    ]),
    confirmPassword: new FormControl("", [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(32),
    ]),
  });

  private readonly lang: string = localStorage.getItem("lang") || "fr";

  protected confirmPasswordError: boolean = false;
  constructor() {
    this.translateService.use(this.lang);
  }

  protected error = new TuiValidationError("Les mots de passes ne correspondents pas");

  protected get passwordError(): TuiValidationError | null {
    return this.confirmPasswordError ? this.error : null;
  }

  protected onSubmit(event: MouseEvent): void {
    event.preventDefault();
    if (this.form.valid) {
      if (this.form.value.password !== this.form.value.confirmPassword) {
        this.confirmPasswordError = true;
      } else {
        this.confirmPasswordError = false;
        const registerInformation: AuthRegister = {
          username: this.form.value.username ?? "",
          email: this.form.value.email ?? "",
          password: this.form.value.password ?? "",
          confirmPassword: this.form.value.confirmPassword ?? "",
        };
        this.authService.sendRegisterRequest(registerInformation).subscribe();
      }
    }
  }
}
