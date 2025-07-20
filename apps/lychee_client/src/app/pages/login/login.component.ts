import { Component } from "@angular/core";
import { NgOptimizedImage } from "@angular/common";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiHeader } from "@taiga-ui/layout";
import { TuiTitle } from "@taiga-ui/core";
import { LoginFormComponent } from "@components/login-form/login-form.component";

@Component({
  selector: "app-login",
  imports: [NgOptimizedImage, TranslatePipe, TuiHeader, TuiTitle, LoginFormComponent],
  templateUrl: "./login.component.html",
  styleUrl: "./login.component.scss",
})
export class LoginComponent {}
