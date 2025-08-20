import { Component } from "@angular/core";
import { TuiAppearance, TuiIcon, TuiTitle } from "@taiga-ui/core";
import { TuiAvatar, TuiBadge, TuiStatus } from "@taiga-ui/kit";
import { TuiCard } from "@taiga-ui/layout";
import { NgClass } from "@angular/common";

type MetricType = "temperature" | "humidity" | "air_pressure";
type MetricColor = "thermometer" | "droplet" | "gauge";
type MetricIcon = "@tui.thermometer" | "@tui.droplet" | "@tui.gauge";

interface Metric {
  id: number;
  type: MetricType;
  label: string;
  value: number;
  last_update: string;
  icon: MetricIcon;
  color: MetricColor;
}

type AlertType = "success" | "warning" | "error" | "info";

interface Alert {
  id: number;
  type: AlertType;
  title: string;
  last_update: string;
  description?: string;
}

@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [TuiAppearance, TuiCard, TuiAvatar, TuiTitle, NgClass, TuiIcon, TuiBadge, TuiStatus],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent {
  // Metric section
  metrics: Metric[] = [
    {
      id: 1,
      type: "temperature",
      label: "Temperature",
      value: 18,
      last_update: "08/07/2025 12:00",
      icon: "@tui.thermometer",
      color: "thermometer",
    },
    {
      id: 2,
      type: "humidity",
      label: "Humidity",
      value: 54,
      last_update: "08/07/2025 08:45",
      icon: "@tui.droplet",
      color: "droplet",
    },
    {
      id: 3,
      type: "air_pressure",
      label: "Air Pressure",
      value: 1013,
      last_update: "08/07/2025 11:26",
      icon: "@tui.gauge",
      color: "gauge",
    },
  ];

  private readonly units: Record<MetricType, string> = {
    temperature: "°C",
    humidity: "%",
    air_pressure: "hPa",
  };

  getUnit(type: MetricType): string {
    return this.units[type];
  }

  // Alert section
  alerts: Alert[] = [
    {
      id: 1,
      type: "success",
      title: "Your carrots have been planted",
      last_update: "08/07/2025",
    },
    {
      id: 2,
      type: "warning",
      title: "Low humidity detected",
      last_update: "08/07/2025",
      description: "Humidity below 50%",
    },
  ];

  readonly badgeAppearance: Record<AlertType, "positive" | "warning" | "negative" | "neutral"> = {
    success: "positive",
    warning: "warning",
    error: "negative",
    info: "neutral",
  };

  readonly badgeLabel: Record<AlertType, string> = {
    success: "Success",
    warning: "Warning",
    error: "Error",
    info: "Info",
  };
}
