import { Component, inject } from "@angular/core";
import { Camera, CameraResultType } from "@capacitor/camera";
import { CropsService } from "@services/crops.service";
import { Http } from "@capacitor-community/http";
@Component({
  selector: "app-crops-capture",
  imports: [],
  templateUrl: "./crops-capture.component.html",
  styleUrl: "./crops-capture.component.scss",
})
export class CropsCaptureComponent {
  identifiedPlants: any[] = [];
  apiKey = "2b100QmmsiK4AzC4vTpJ7Ve";
  private readonly cropsService = inject(CropsService);

  async captureAndIdentify() {
    const base64 = await this.takePicture();
    try {
      const res = await this.cropsService.identifyPlant(base64, this.apiKey);
      this.identifiedPlants = res.results || [];
      console.log("Pl@ntNet response:", res);
    } catch (e) {
      console.error("Erreur identification:", e);
    }
  }

  async takePicture(): Promise<string> {
    const photo = await Camera.getPhoto({
      quality: 90,
      resultType: CameraResultType.Base64,
    });
    return photo.base64String!;
  }
}
