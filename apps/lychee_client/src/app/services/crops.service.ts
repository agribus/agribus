import { inject, Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Camera, CameraResultType } from "@capacitor/camera";
import { environment } from "@environment/environment";

@Injectable({
  providedIn: "root",
})
export class CropsService {
  private apiUrl = "https://my-api.plantnet.org/v2/identify/all";
  identifiedPlants: any[] = [];
  commonName = "";
  private readonly http = inject(HttpClient);

  constructor() {}

  async captureAndIdentify() {
    const base64 = await this.takePicture();
    this.identifyPlant(base64, environment.planetApi).subscribe({
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

          console.log(bestResult);
          console.log(commonName);
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

  identifyPlant(base64Image: string, apiKey: string) {
    const formData = new FormData();
    formData.append("images", this.base64ToBlob(base64Image));
    formData.append("organs", "leaf");

    let params = new HttpParams().set("api-key", apiKey);
    params = params.set("lang", "fr");

    return this.http.post(this.apiUrl, formData, { params });
  }

  private base64ToBlob(base64: string): Blob {
    const byteString = window.atob(base64);
    const arrayBuffer = new ArrayBuffer(byteString.length);
    const intArray = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
      intArray[i] = byteString.charCodeAt(i);
    }
    return new Blob([intArray], { type: "image/jpeg" });
  }
}
