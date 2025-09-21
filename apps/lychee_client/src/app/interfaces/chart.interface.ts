export interface MetricPoint {
  /** "yyyy-MM-dd" */
  date: string;
  /** null when no data for that day */
  value: number | null;
}

export interface MetricSeries {
  unit: string;
  yMin: number;
  yMax: number;
  points: MetricPoint[];
}

export interface ChartMetrics {
  temperature: MetricSeries;
  humidity: MetricSeries;
  airPressure: MetricSeries;
}

export interface ChartRange {
  /** "yyyy-MM-dd" */
  from: string;
  /** "yyyy-MM-dd" */
  to: string;
}

export interface ChartTimeseries {
  range: ChartRange;
  metrics: ChartMetrics;
}

export interface ChartTimeseriesResponse {
  chartTimeseries: ChartTimeseries;
}
