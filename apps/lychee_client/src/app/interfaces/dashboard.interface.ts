export interface Metric {
  value: number;
  unit: string;
  timestamp: string;
}

export interface SummaryAggregates {
  metrics: {
    temperature: Metric;
    humidity: Metric;
    pressure: Metric;
  };
}

export interface Sensor {
  sourceAddress: string;
  name: string;
  last: {
    timestamp: string;
    temperature: Metric;
    humidity: Metric;
    pressure: Metric;
  };
}

export interface SensorData {
  summaryAggregates: SummaryAggregates;
  sensors: Sensor[];
}
