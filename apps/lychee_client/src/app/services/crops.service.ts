import { inject, Injectable } from "@angular/core";
import { Http } from "@capacitor-community/http";
@Injectable({
  providedIn: "root",
})
export class CropsService {
  private apiUrl = "https://my-api.plantnet.org/v2/identify/all";

  constructor() {}

  private base64ToBlob(base64: string): Blob {
    const byteCharacters = atob(base64);
    const byteNumbers = new Array(byteCharacters.length)
      .fill(0)
      .map((_, i) => byteCharacters.charCodeAt(i));
    const byteArray = new Uint8Array(byteNumbers);
    return new Blob([byteArray], { type: "image/jpeg" });
  }

  async identifyPlant(base64Image: string, apiKey: string): Promise<any> {
    // Pl@ntNet attend un form-data avec le fichier image (key = 'images')

    // Malheureusement, Capacitor HTTP ne supporte pas FormData directement,
    // il faut construire le body en multipart/form-data "manuellement".

    const boundary = "----WebKitFormBoundary7MA4YWxkTrZu0gW";

    const blob = this.base64ToBlob(base64Image);
    const reader = new FileReader();

    return new Promise((resolve, reject) => {
      reader.onload = async () => {
        const fileData = new Uint8Array(reader.result as ArrayBuffer);
        let body = "";

        // Construire le multipart form-data body
        body += `--${boundary}\r\n`;
        body += `Content-Disposition: form-data; name="images"; filename="plant.jpg"\r\n`;
        body += `Content-Type: image/jpeg\r\n\r\n`;

        // Convert fileData to string for concatenation — mais ça casse les bytes binaires, donc on fait autrement
        // Capacitor HTTP accepte un base64 string directement dans data

        body = ""; // On va plutôt envoyer via base64 string dans data

        // En fait, on peut envoyer directement le base64 dans data sous la clé 'images' encodée

        try {
          const response = await Http.request({
            method: "POST",
            url: `${this.apiUrl}?api-key=${apiKey}`,
            headers: {
              "Content-Type": "multipart/form-data; boundary=" + boundary,
            },
            // Pas possible de passer formData, donc on passe data sous forme string JSON ou base64 (selon API)
            // Alternative : envoyer en JSON si Pl@ntNet supporte

            data: {
              images: base64Image,
              organs: ["leaf"],
            },
          });
          resolve(response.data);
        } catch (error) {
          reject(error);
        }
      };

      reader.onerror = err => {
        reject(err);
      };

      reader.readAsArrayBuffer(blob);
    });
  }
}
