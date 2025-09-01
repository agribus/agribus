export interface Sensor {
  id: number;
  sourceAddress?: string;
  name: string;
  last_update?: string;
  temperature?: number;
  humidity?: number;
  air_pressure?: number;
}
