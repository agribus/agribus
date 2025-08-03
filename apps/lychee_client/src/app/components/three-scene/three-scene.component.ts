import { Component, ElementRef, inject, OnInit, ViewChild } from "@angular/core";
import { ThreeSceneService } from "@services/three-scene/three-scene.service";

@Component({
  selector: "app-three-scene",
  imports: [],
  templateUrl: "./three-scene.component.html",
  styleUrl: "./three-scene.component.scss",
})
export class ThreeSceneComponent implements OnInit {
  @ViewChild("greenhouseCanva", { static: true })
  public greenhouseCanva: ElementRef<HTMLElement>;

  private readonly threeSceneService = inject(ThreeSceneService);

  ngOnInit(): void {
    this.threeSceneService.createScene(this.greenhouseCanva);
    setTimeout(() => {
      this.threeSceneService.plantVegetable("carrot", 5);
    }, 1000);
  }
}
