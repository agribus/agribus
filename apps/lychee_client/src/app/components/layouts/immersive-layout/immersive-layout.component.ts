import { Component } from "@angular/core";
import { HeaderComponent } from "@components/ui/header/header.component";
import { NavbarComponent } from "@components/ui/navbar/navbar.component";
import { RouterOutlet } from "@angular/router";
import { HeaderType } from "@enums/header-type";

@Component({
  selector: "app-immersive-layout",
  imports: [HeaderComponent, NavbarComponent, RouterOutlet],
  templateUrl: "./immersive-layout.component.html",
  styleUrl: "./immersive-layout.component.scss",
})
export class ImmersiveLayoutComponent {
  protected HEADER_DEFAULT = HeaderType.Default;
}
