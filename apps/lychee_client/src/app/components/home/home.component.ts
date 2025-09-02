import { Component, inject } from "@angular/core";
import { GreenhouseSceneComponent } from "@components/greenhouse-scene/greenhouse-scene.component";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { TuiButton } from "@taiga-ui/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-home",
  imports: [GreenhouseSceneComponent, TuiButton],
  templateUrl: "./home.component.html",
  styleUrl: "./home.component.scss",
})
export class HomeComponent {
  private readonly greenhouseService = inject(GreenhouseService);
  private readonly router = inject(Router);

  get userHaveGreenhouse(): boolean {
    return this.greenhouseService.greenhouses().length > 0;
  }

  navigateToGreenhouseForm() {
    this.router.navigate(["/greenhouse/create"]);
  }
}
