import { AsyncPipe, NgOptimizedImage } from "@angular/common";
import { ChangeDetectionStrategy, Component } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";

import {
  TuiAppearance,
  TuiButton,
  TuiError,
  TuiIcon,
  TuiTextfield,
  TuiTitle,
} from "@taiga-ui/core";
import { TuiFieldErrorPipe, tuiValidationErrorsProvider } from "@taiga-ui/kit";
import { TuiCardLarge, TuiForm, TuiHeader } from "@taiga-ui/layout";
import { TuiValidationError } from "@taiga-ui/cdk";

@Component({
  selector: "app-login-form",
  imports: [
    AsyncPipe,
    ReactiveFormsModule,
    TuiAppearance,
    TuiButton,
    TuiCardLarge,
    TuiError,
    TuiFieldErrorPipe,
    TuiForm,
    TuiHeader,
    TuiIcon,
    TuiTextfield,
    TuiTitle,
    NgOptimizedImage,
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
  protected enabled = false;

  protected error = new TuiValidationError("test");

  protected get computedError(): TuiValidationError | null {
    return this.enabled ? this.error : null;
  }

  protected readonly form = new FormGroup({
    email: new FormControl("", Validators.required),
    password: new FormControl("", Validators.required),
  });
}
