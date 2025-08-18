import { Component } from "@angular/core";
import { HeaderComponent } from "@components/ui/header/header.component";
import { RouterOutlet } from "@angular/router";
import { HeaderType } from "@enums/header-type";

@Component({
  selector: "app-settings-layout",
  imports: [HeaderComponent, RouterOutlet],
  templateUrl: "./settings-layout.component.html",
  styleUrl: "./settings-layout.component.scss",
})
export class SettingsLayoutComponent {
  protected HEADER_SETTINGS = HeaderType.Settings;
}
