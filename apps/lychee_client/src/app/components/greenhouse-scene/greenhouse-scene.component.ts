import {
  Component,
  effect,
  ElementRef,
  inject,
  Injector,
  OnInit,
  runInInjectionContext,
  ViewChild,
} from "@angular/core";
import { GreenhouseSceneService } from "@services/greenhouse-scene/greenhouse-scene.service";
import { Crop } from "@interfaces/crop.interface";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { TranslatePipe } from "@ngx-translate/core";
import { TuiButton } from "@taiga-ui/core";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { ActivatedRoute } from "@angular/router";
import { firstValueFrom } from "rxjs";

@Component({
  selector: "app-greenhouse-scene",
  imports: [TranslatePipe, TuiButton],
  templateUrl: "./greenhouse-scene.component.html",
  styleUrl: "./greenhouse-scene.component.scss",
})
export class GreenhouseSceneComponent implements OnInit {
  @ViewChild("greenhouseCanva", { static: true })
  public greenhouseCanva!: ElementRef<HTMLElement>;

  private readonly threeSceneService = inject(GreenhouseSceneService);
  private readonly greenhouseService = inject(GreenhouseService);
  private readonly injector = inject(Injector);

  private readonly carrotCrop: Crop = {
    commonName: "Carotte",
    commonNames: ["Carotte", "Carrot", "Zanahoria"],
    scientificName: "Daucus carota subsp. sativus",
    score: 85,
    quantity: 7,
    plantingDate: new Date("2025-03-01"),
    cropGrowthConditions: {
      atmosphericHumidity: 23.4,
      miniumTemperature: 15.6,
      maximumTemperature: 25.6,
    },
  };

  private readonly greenhouse: Greenhouse = {
    id: "guid-001",
    name: "Serre expérimentale",
    city: "Paris",
    country: "France",
    crops: [this.carrotCrop],
    sensors: [],
  };

  async ngOnInit(): Promise<void> {
    await this.threeSceneService.createScene(this.greenhouseCanva);

    runInInjectionContext(this.injector, () => {
      let lastId: string | null = null;

      effect(async () => {
        const serre = this.greenhouseService.selectedSerre();
        if (!serre?.id) return;

        if (serre.id !== lastId) {
          this.threeSceneService.clearCrops();
          this.greenhouseService.loadGreenhouseById(serre.id);
          lastId = serre.id;

          const greenhouse: Greenhouse = await firstValueFrom(
            this.greenhouseService.loadGreenhouseById(serre.id)
          );
          await this.threeSceneService.plantCropsFromGreenhouse(greenhouse);
        }
      });
    });
  }

  get isInsideView(): boolean {
    return this.threeSceneService.isInsideView();
  }

  onReturnToDefault(): void {
    this.threeSceneService.returnToDefaultCamera();
  }
}
