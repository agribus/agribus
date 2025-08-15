import { Component } from "@angular/core";
import { GreenhouseFormComponent } from "@components/greenhouses/greenhouse-form/greenhouse-form.component";

@Component({
  selector: "app-greenhouse-create",
  imports: [GreenhouseFormComponent],
  templateUrl: "./greenhouse-create.component.html",
  styleUrl: "./greenhouse-create.component.scss",
})
export class GreenhouseCreateComponent {}
