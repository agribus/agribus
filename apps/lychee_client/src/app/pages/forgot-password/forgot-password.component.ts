import { Component } from "@angular/core";
import { NgOptimizedImage } from "@angular/common";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiTitle } from "@taiga-ui/core";
import { ForgotPasswordFormComponent } from "@components/forgot-password-form/forgot-password-form.component";

@Component({
  selector: "app-forgot-password",
  imports: [NgOptimizedImage, TranslatePipe, TuiTitle, ForgotPasswordFormComponent],
  templateUrl: "./forgot-password.component.html",
  styleUrl: "./forgot-password.component.scss",
})
export class ForgotPasswordComponent {}
