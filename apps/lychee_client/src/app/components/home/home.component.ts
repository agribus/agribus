import { Component } from "@angular/core";
import { HelloWorldComponent } from "@components/hello-world/hello-world.component";

@Component({
  selector: "app-home",
  imports: [HelloWorldComponent],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.scss",
})
export class HomeComponent {}
