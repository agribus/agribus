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
  protected sections = [
    {
      content: "Latest Measures",
      order: new Map(),
      children: [
        { content: "Temperature", icon: "thermometer", data: "24" },
        { content: "Humidity", icon: "droplet", data: "24" },
        { content: "Pression atmosphérique", icon: "gauge", data: "24" },
      ],
    },
    { content: "Latest Alerts", order: new Map() },
    { content: "Charts", order: new Map() },
  ];

  protected order = new Map();
  // protected items = [
  //   { w: 1, h: 1, isChart: false, content: "Temperature", icon: "thermometer" },
  //   { w: 1, h: 1, isChart: false, content: "Humidity", icon: "droplet" },
  //   { w: 1, h: 1, isChart: false, content: "Air pressure", icon: "gauge" },
  //   { w: 1, h: 1, isChart: false, content: "Item 4" },
  //   { w: 3, h: 1, isChart: false, content: "Item 5" },
  //   { w: 1, h: 1, isChart: false, content: "Item 6" },
  //   { w: 3, h: 2, isChart: true, content: "Item 7" },
  //   { w: 1, h: 1, isChart: false, content: "Item 8" },
  //   { w: 1, h: 1, isChart: false, content: "Item 9" },
  // ];
  //
  // protected order = new Map();

  protected readonly value: readonly TuiPoint[] = [
    [50, 50],
    [100, 75],
    [150, 50],
    [200, 150],
    [250, 155],
    [300, 190],
    [350, 90],
  ];
}
