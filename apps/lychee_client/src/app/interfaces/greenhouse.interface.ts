import { Sensor } from "@interfaces/sensor.interface";
import { Crop } from "@interfaces/crop.interface";

export interface Greenhouse {
  readonly id: string;
  readonly name: string;
  readonly city: string;
  readonly country: string;
  readonly crops: Crop[];
  readonly sensors: Sensor[];
}
