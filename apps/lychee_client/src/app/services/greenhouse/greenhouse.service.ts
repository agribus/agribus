import { inject, Injectable } from "@angular/core";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { environment } from "@environment/environment";
import { Crop } from "@interfaces/crop.interface";
import { Sensor } from "@interfaces/sensor.interface";
import { HttpClient } from "@angular/common/http";

@Injectable({
  providedIn: "root",
})
export class GreenhouseService {
  private readonly http = inject(HttpClient);

  protected readonly greenhouses: Greenhouse[] = [
    { id: 42, name: "Serre 1" },
    { id: 237, name: "Serre 2" },
    { id: 666, name: "Serre 3" },
  ];

  getGreenhouses(): Greenhouse[] {
    return this.greenhouses;
  }

  public createGreenhouse(
    name: string,
    city: string,
    country: string,
    crops: Crop[],
    sensors: Sensor[]
  ) {
    return this.http.post(environment.apiUrl + "/greenhouses", {
      name: name,
      city: city,
      country: country,
      crops: crops,
      sensors: sensors,
    });
  }
}
