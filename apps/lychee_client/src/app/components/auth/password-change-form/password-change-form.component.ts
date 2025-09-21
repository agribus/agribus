import { Component, inject, ChangeDetectionStrategy } from "@angular/core";
import {
  ReactiveFormsModule,
  Validators,
  FormGroup,
  FormControl,
  FormsModule,
} from "@angular/forms";
import { TuiValidationError } from "@taiga-ui/cdk";
import { TuiFieldErrorPipe, TuiPassword, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { TuiAlertService, TuiButton, TuiError, TuiIcon, TuiTextfield } from "@taiga-ui/core";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { AuthPasswordChange, AuthResponse } from "@interfaces/auth.interface";
import { AuthService } from "@services/auth/auth.service";
import { Router } from "@angular/router";
import { AsyncPipe } from "@angular/common";

@Component({
  selector: "app-password-change-form",
  imports: [
    ReactiveFormsModule,
    TuiButton,
    TuiTextfield,
    TuiIcon,
    TuiFieldErrorPipe,
    TuiPassword,
    TranslatePipe,
    FormsModule,
    AsyncPipe,
    TuiError,
  ],
  templateUrl: "./password-change-form.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrl: "./password-change-form.component.scss",
  providers: [
    tuiValidationErrorsProvider({
      required: "Champ requis.",
      minlength: "Le mot de passe doit faire au moins 8 caractères.",
      maxlength: "Le mot de passe ne doit pas dépasser 32 caractères.",
    }),
  ],
})
export class PasswordChangeFormComponent {
  private readonly translateService: TranslateService = inject(TranslateService);
  private readonly authService: AuthService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly alerts = inject(TuiAlertService);
  protected readonly form = new FormGroup({
    currentPassword: new FormControl("", [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(32),
    ]),
    newPassword: new FormControl("", [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(32),
    ]),
    confirmNewPassword: new FormControl("", [
      Validators.required,
      Validators.minLength(8),
      Validators.maxLength(32),
    ]),
  });

  public messageError: string = "";
  public isError: boolean = false;

  protected confirmNewPasswordError: boolean = false;

  protected error = new TuiValidationError("Les mots de passes ne correspondents pas");

  protected get passwordError(): TuiValidationError | null {
    return this.confirmNewPasswordError ? this.error : null;
  }

  protected onSubmit(event: MouseEvent): void {
    event.preventDefault();
    if (this.form.valid) {
      if (this.form.value.newPassword !== this.form.value.confirmNewPassword) {
        this.confirmNewPasswordError = true;
      } else {
        this.confirmNewPasswordError = false;
        const passwordChangeInformation: AuthPasswordChange = {
          currentPassword: this.form.value.currentPassword ?? "",
          newPassword: this.form.value.newPassword ?? "",
          confirmNewPassword: this.form.value.confirmNewPassword ?? "",
        };
        console.log(passwordChangeInformation);
        this.authService.sendPasswordChangeRequest(passwordChangeInformation).subscribe({
          next: (response: AuthResponse) => {
            if (response.success) {
              this.router.navigate(["/settings"]);
            }
          },
          error: error => {
            console.log(error);
            if (error.status === 400) {
              this.isError = true;
              this.messageError = this.translateService.instant("shared.alerts.errorCreds");
            } else {
              this.alerts
                .open(error.message, {
                  label: this.translateService.instant("shared.alerts.error"),
                  appearance: "error",
                })
                .subscribe();
            }
          },
        });
      }
    }
  }
}
