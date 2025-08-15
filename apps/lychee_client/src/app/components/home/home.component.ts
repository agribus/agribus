import { Component } from "@angular/core";
import { GreenhouseSceneComponent } from "@components/greenhouse-scene/greenhouse-scene.component";

@Component({
  selector: "app-home",
  imports: [GreenhouseSceneComponent],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.scss",
})
export class HomeComponent {}
