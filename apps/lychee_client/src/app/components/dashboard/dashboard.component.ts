import { Component } from "@angular/core";
import { TuiIcon, TuiPoint } from "@taiga-ui/core";
import { TuiTile, TuiTileHandle, TuiTiles } from "@taiga-ui/kit";
import { TuiAxes, TuiLineChart } from "@taiga-ui/addon-charts";

@Component({
  selector: "app-dashboard",
  standalone: true,
  imports: [TuiIcon, TuiTile, TuiTileHandle, TuiTiles, TuiAxes, TuiLineChart],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent {
  protected order = new Map();
  protected measuresOrder = new Map();
  protected sensorsOrder = new Map();
  protected alertsOrder = new Map();
  protected chartsOrder = new Map();

  protected readonly value: readonly TuiPoint[] = [
    [50, 50],
    [100, 75],
    [150, 50],
    [200, 150],
    [250, 155],
    [300, 190],
    [350, 90],
  ];

  protected measures = [
    { content: "Temperature", icon: "thermometer", data: "24", lastUpdateDate: "08/07/2025 12:00" },
    { content: "Humidity", icon: "droplet", data: "65", lastUpdateDate: "08/07/2025 08:45" },
    { content: "Air Pressure", icon: "gauge", data: "1013", lastUpdateDate: "08/07/2025 11:11" },
  ];

  protected alerts = [
    {
      content: "Low Temperature Detected",
      date: "08/07/2025",
      description: "temperature below 20°C",
    },
    { content: "Low Humidity Detected", date: "08/07/2025", description: "humidity below 50%" },
  ];

  protected charts = [{ id: "chart1", data: this.value }];

  protected sensors = [
    { id: "1", temperature: "13,0", humidity: "53", air_pressure: "1025" },
    { id: "2", temperature: "13,2", humidity: "54", air_pressure: "1025" },
    { id: "3", temperature: "12,9", humidity: "51", air_pressure: "1025" },
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
