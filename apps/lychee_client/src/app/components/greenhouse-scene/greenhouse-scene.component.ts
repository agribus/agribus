import { Component, ElementRef, inject, OnInit, ViewChild } from "@angular/core";
import { GreenhouseSceneService } from "@services/greenhouse-scene/greenhouse-scene.service";

@Component({
  selector: "app-greenhouse-scene",
  imports: [],
  templateUrl: "./greenhouse-scene.component.html",
  styleUrl: "./greenhouse-scene.component.scss",
})
export class GreenhouseSceneComponent implements OnInit {
  @ViewChild("greenhouseCanva", { static: true })
  public greenhouseCanva: ElementRef<HTMLElement>;

  private readonly threeSceneService = inject(GreenhouseSceneService);

  ngOnInit(): void {
    this.threeSceneService.createScene(this.greenhouseCanva);
    setTimeout(() => {
      this.threeSceneService.plantVegetable("carrot", 5);
    }, 1000);
  }
}
