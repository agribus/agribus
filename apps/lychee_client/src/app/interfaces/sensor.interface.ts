export interface Sensor {
  id: number;
  sourceAddress: string;
  name: string;
  sensorModel?: string;
  isActive?: boolean;
}
