import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { HeaderComponent } from "@components/ui/header/header.component";
import { HeaderType } from "@enums/header-type";

@Component({
  selector: "app-auth-layout",
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: "./auth-layout.component.html",
  styleUrl: "./auth-layout.component.scss",
})
export class AuthLayoutComponent {
  protected HEADER_NONE = HeaderType.None;
}
