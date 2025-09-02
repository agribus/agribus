export interface Sensor {
  id: number;
  sourceAddress?: string;
  name: string;
  sensorModel?: string;
  isActive?: boolean;
  last_update?: string;
  temperature?: number;
  humidity?: number;
  air_pressure?: number;
}
