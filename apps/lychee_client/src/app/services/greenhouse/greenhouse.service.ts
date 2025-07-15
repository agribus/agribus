import { Injectable } from "@angular/core";
import { Greenhouse } from "@interfaces/greenhouse.interface";

@Injectable({
  providedIn: "root",
})
export class GreenhouseService {
  protected readonly greenhouses: Greenhouse[] = [
    { id: 42, name: "Serre 1" },
    { id: 237, name: "Serre 2" },
    { id: 666, name: "Serre 3" },
  ];

  getGreenhouses(): Greenhouse[] {
    return this.greenhouses;
  }
}
