import { Component } from "@angular/core";
import { HelloWorldComponent } from "@components/hello-world/hello-world.component";
import { ThreeSceneComponent } from "@components/three-scene/three-scene.component";

@Component({
  selector: "app-home",
  imports: [ThreeSceneComponent],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.scss",
})
export class HomeComponent {}
