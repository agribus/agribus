import { Component } from "@angular/core";
import { GreenhouseFormComponent } from "@components/greenhouse-form/greenhouse-form.component";

@Component({
  selector: "app-greenhouse-settings",
  imports: [GreenhouseFormComponent],
  templateUrl: "./greenhouse-settings.component.html",
  styleUrl: "./greenhouse-settings.component.scss",
})
export class GreenhouseSettingsComponent {}
