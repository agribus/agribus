import { AsyncPipe } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { TuiButton, TuiError, TuiTextfield } from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { AuthLogin, AuthResponse } from "@interfaces/auth.interface";
import { AuthService } from "@services/auth/auth.service";

@Component({
  selector: "app-login-form",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiButton,
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
  private readonly translateService: TranslateService = inject(TranslateService);
  private readonly authService: AuthService = inject(AuthService);
  private router = inject(Router);
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

      this.authService.SendLoginRequest(loginInformation).subscribe({
        next: (response: AuthResponse) => {
          if (response.success) {
            this.router.navigate(["/home"]);
          }
        },
        error: (error: Error) => console.log(error),
      });
    }
  }
}
