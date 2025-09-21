import { Component } from "@angular/core";
import { PasswordChangeFormComponent } from "@components/auth/password-change-form/password-change-form.component";

@Component({
  selector: "app-password-change",
  imports: [PasswordChangeFormComponent],
  templateUrl: "./password-change.component.html",
  styleUrl: "./password-change.component.scss",
})
export class PasswordChangeComponent {}
