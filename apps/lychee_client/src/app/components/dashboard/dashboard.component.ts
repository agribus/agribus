import { Component } from "@angular/core";
import { TuiAppearance, TuiTitle } from "@taiga-ui/core";
import { TuiAvatar } from "@taiga-ui/kit";
import { TuiCard } from "@taiga-ui/layout";
import { NgClass } from "@angular/common";

@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [TuiAppearance, TuiCard, TuiAvatar, TuiTitle, NgClass],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent {
  metrics = [
    {
      id: 1,
      type: "temperature",
      value: "18",
      updated: "08/07/2025 12:00",
      icon: "@tui.thermometer",
      color: "thermometer",
    },
    {
      id: 2,
      type: "humidity",
      value: "54",
      updated: "08/07/2025 08:45",
      icon: "@tui.droplet",
      color: "droplet",
    },
    {
      id: 3,
      type: "air_pressure",
      value: "1013",
      updated: "08/07/2025 11:26",
      icon: "@tui.gauge",
      color: "gauge",
    },
  ];

  private units: Record<string, string> = {
    temperature: "°C",
    humidity: "%",
    air_pressure: "hPa",
  };

  getUnit(type: keyof typeof this.units): string {
    return this.units[type] || "";
  }
}
