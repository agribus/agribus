import { Component } from "@angular/core";
import { HeaderComponent } from "@components/ui/header/header.component";
import { RouterOutlet } from "@angular/router";
import { HeaderType } from "@enums/header-type";
import { NavbarComponent } from "@components/ui/navbar/navbar.component";

@Component({
  selector: "app-main-layout",
  imports: [HeaderComponent, RouterOutlet, NavbarComponent],
  templateUrl: "./main-layout.component.html",
  styleUrl: "./main-layout.component.scss",
})
export class MainLayoutComponent {
  protected HEADER_DEFAULT = HeaderType.Default;
}
