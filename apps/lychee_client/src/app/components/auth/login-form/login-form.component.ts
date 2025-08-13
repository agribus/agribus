import { AsyncPipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import {
  TuiAlertService,
  TuiButton,
  TuiError,
  TuiNotification,
  TuiTextfield,
} from "@taiga-ui/core";
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
    TuiNotification,
  ],
  templateUrl: "./login-form.component.html",
  styleUrl: "./login-form.component.scss",
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis",
      email: "Veuillez entrez une adresse mail valide",
      minlength: "Le mot de passe doit faire au moins 8 caractères",
      maxlength: "Le mot de passe ne doit pas dépasser 32 caractères"
    }),
  ],
})
export class LoginFormComponent {
  private readonly translateService: TranslateService = inject(TranslateService);
  private readonly authService: AuthService = inject(AuthService);
  private router = inject(Router);
  protected readonly form = new FormGroup({
    email: new FormControl("", [Validators.required, Validators.email]),
    password: new FormControl("", [Validators.required, Validators.minLength(8),
      Validators.maxLength(32)]),
  });
  private readonly lang = localStorage.getItem("lang") || "fr";
  private readonly alerts = inject(TuiAlertService);

  public messageError: string = "";
  public isError: boolean = false;

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
        error: error => {
          if (error.status === 400) {
            this.isError = true;
            this.messageError = this.translateService.instant("components.ui.alert.errorCreds");
          } else {
            this.alerts
              .open(error.message, {
                label: this.translateService.instant("components.ui.alert.error"),
                appearance: "error",
              })
              .subscribe();
          }
        },
      });
    }
  }
}
