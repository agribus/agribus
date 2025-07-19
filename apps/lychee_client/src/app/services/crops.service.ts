import { inject, Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class CropsService {
  private apiUrl = "https://my-api.plantnet.org/v2/identify/all";

  private readonly http = inject(HttpClient);

  constructor() {}

  identifyPlant(base64Image: string, apiKey: string) {
    const formData = new FormData();
    formData.append("images", this.base64ToBlob(base64Image));
    formData.append("organs", "leaf");

    let params = new HttpParams().set("api-key", apiKey);
    params = params.set("lang", "fr");

    return this.http.post(this.apiUrl, formData, { params });
  }

  getLanguages(apiKey: string) {
    const params = new HttpParams().set("api-key", apiKey);

    return this.http.get("https://my-api.plantnet.org/v2/languages", { params });
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
