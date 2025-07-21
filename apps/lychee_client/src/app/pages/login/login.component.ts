import { Component } from "@angular/core";
import { NgOptimizedImage } from "@angular/common";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiTitle } from "@taiga-ui/core";
import { LoginFormComponent } from "@components/login-form/login-form.component";

@Component({
  selector: "app-login",
  imports: [NgOptimizedImage, TranslatePipe, TuiTitle, LoginFormComponent],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent {}
