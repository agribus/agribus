import { Component } from "@angular/core";
import { RouterOutlet } from "@angular/router";
import { HeaderComponent } from "@components/ui/header/header.component";
import { HeaderType } from "@enums/header-type";

@Component({
  selector: "app-minimal-layout",
  imports: [RouterOutlet, HeaderComponent],
  templateUrl: "./minimal-layout.component.html",
  styleUrl: "./minimal-layout.component.scss",
})
export class MinimalLayoutComponent {
  protected HEADER_NONE = HeaderType.None;
}
