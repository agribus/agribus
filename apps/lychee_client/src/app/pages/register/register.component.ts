import { Component } from "@angular/core";
import { NgOptimizedImage } from "@angular/common";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiTitle } from "@taiga-ui/core";
import { RegisterFormComponent } from "@components/register-form/register-form.component";

@Component({
  selector: "app-register",
  imports: [RegisterFormComponent, NgOptimizedImage, TranslatePipe, TuiTitle],
  templateUrl: "./register.component.html",
  styleUrl: "./register.component.scss",
})
export class RegisterComponent {}
