import { inject, Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Camera, CameraResultType } from "@capacitor/camera";
import { environment } from "@environment/environment";
import { from, map, Observable, switchMap } from "rxjs";
import { Crop } from "@interfaces/crop.interface";

@Injectable({
  providedIn: "root",
})
export class CropsService {
  private readonly http = inject(HttpClient);

  /**
   * Captures a photo and identifies the plant
   * @returns Observable<Crop[]> The 3 best matches
   */
  identifyPlantFromCamera(): Observable<Crop[]> {
    return from(this.takePicture()).pipe(
      switchMap(base64 =>
        this.identifyPlant(base64, environment.planetApiKey).pipe(
          map((crops: Crop[]) =>
            crops.map(crop => ({
              ...crop,
              date_plantation: new Date(),
              imageUrl: `data:image/jpeg;base64,${base64}`,
            }))
          )
        )
      )
    );
  }

  /**
   * Takes a photo using the device camera
   */
  private async takePicture(): Promise<string> {
    const photo = await Camera.getPhoto({
      quality: 90,
      resultType: CameraResultType.Base64,
      promptLabelHeader: "Prendre une photo",
      promptLabelPhoto: "Choisir une photo",
      promptLabelPicture: "Prendre une photo",
    });
    return photo.base64String!;
  }

  /**
   * Sends the photo to the API for identification
   */
  private identifyPlant(base64Image: string, apiKey: string): Observable<any> {
    const formData = new FormData();
    formData.append("images", this.base64ToBlob(base64Image));
    formData.append("organs", "leaf");

    const params = new HttpParams().set("api-key", apiKey).set("lang", "fr");

    return this.http
      .post(environment.planetApiUrl + "/v2/identify/all", formData, { params })
      .pipe(map((response: any) => this.processPlantResponse(response)));
  }

  /**
   * Processes the API response to extract relevant information
   */
  private processPlantResponse(response: any): Crop[] {
    if (!response?.results?.length) {
      return [];
    }

    return response.results
      .sort((a: any, b: any) => b.score - a.score)
      .slice(0, 3)
      .map((result: any) => ({
        commonName: result.species.commonNames?.[0] || result.species.scientificNameWithoutAuthor,
        commonNames: result.species.commonNames,
        scientificName: result.species.scientificNameWithoutAuthor,
        score: Math.round(result.score * 100) / 100,
        quantity: 0,
        date_plantation: new Date(),
      }));
  }

  /**
   * Converts a base64 image to Blob
   */
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
