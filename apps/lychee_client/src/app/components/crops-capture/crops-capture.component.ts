import { Component, inject } from "@angular/core";
import { Camera, CameraResultType } from "@capacitor/camera";
import { CropsService } from "@services/crops.service";
import { NgForOf, PercentPipe } from "@angular/common";

@Component({
  selector: "app-crops-capture",
  imports: [PercentPipe, NgForOf],
  templateUrl: "./crops-capture.component.html",
  styleUrl: "./crops-capture.component.scss",
})
export class CropsCaptureComponent {
  identifiedPlants: any[] = [];
  apiKey = "2b100QmmsiK4AzC4vTpJ7Ve";
  commonName = "";
  private readonly cropsService = inject(CropsService);

  async captureAndIdentify() {
    const base64 = await this.takePicture();
    this.cropsService.identifyPlant(base64, this.apiKey).subscribe({
      next: (res: any) => {
        this.identifiedPlants = res.results || [];

        if (this.identifiedPlants.length > 0) {
          this.identifiedPlants = (res.results || [])
            .sort((a: any, b: any) => b.score - a.score)
            .slice(0, 3);
          const bestResult = this.identifiedPlants[0];

          const commonNames = bestResult.species.commonNames || [];
          const commonName =
            commonNames.length > 0
              ? commonNames[0]
              : bestResult.species.scientificNameWithoutAuthor || "Nom non disponible";

          this.commonName = commonName;
        }
      },
      error: err => {
        console.error("Erreur identification:", err);
      },
    });
  }

  async takePicture(): Promise<string> {
    const photo = await Camera.getPhoto({
      quality: 90,
      resultType: CameraResultType.Base64,
    });
    return photo.base64String!;
  }
}
