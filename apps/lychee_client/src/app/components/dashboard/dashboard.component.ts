import { Component, inject, Injector, INJECTOR } from "@angular/core";
import { AsyncPipe, NgClass } from "@angular/common";
import { FormsModule, FormControl } from "@angular/forms";
import { combineLatest, map, Observable } from "rxjs";

import { TuiCard } from "@taiga-ui/layout";
import { TUI_MONTHS, TuiAppearance, TuiButton, TuiDialogService, TuiTitle } from "@taiga-ui/core";
import { TUI_CALENDAR_DATE_STREAM, TuiAvatar, TuiBadge, TuiStatus } from "@taiga-ui/kit";
import { TuiMobileCalendarDropdown } from "@taiga-ui/addon-mobile";
import { tuiControlValue, TuiDay, TuiDayRange } from "@taiga-ui/cdk";
import { PolymorpheusComponent } from "@taiga-ui/polymorpheus";
import { ChartDashboard } from "../../chart-dashboard/chart-dashboard";

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
  imports: [
    TuiAppearance,
    TuiCard,
    TuiAvatar,
    TuiTitle,
    NgClass,
    TuiBadge,
    TuiStatus,
    TuiButton,
    FormsModule,
    AsyncPipe,
    ChartDashboard,
    ChartDashboard,
  ],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent {
  // Metrics section
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

  // Alerts section
  alerts: Alert[] = [
    { id: 1, type: "success", title: "Your carrots have been planted", last_update: "08/07/2025" },
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

  // Calendar
  private readonly dialogs = inject(TuiDialogService);
  private readonly injector = inject(INJECTOR);
  private readonly months$ = inject(TUI_MONTHS);

  private readonly control = new FormControl<TuiDayRange | null>(null);
  get selectedRange(): TuiDayRange | null {
    return this.control.value;
  }

  // Demo data chart
  private readonly start = new TuiDay(2025, 0, 1);
  private readonly DAYS = 365;

  protected readonly tempData: ReadonlyArray<[TuiDay, number]> = (() => {
    const arr: Array<[TuiDay, number]> = [];
    for (let i = 0; i < this.DAYS; i++) {
      const d = this.start.append({ day: i });
      let v = 22 + 10 * Math.sin(i / 7) + (Math.random() * 2 - 1);
      if (i >= 20 && i <= 26) v += 15;
      if (i >= 70 && i <= 73) v -= 8;
      v = this.clamp(v, 0, 45);
      arr.push([d, Math.round(v * 10) / 10]);
    }
    return arr;
  })();

  protected readonly humidityData: ReadonlyArray<[TuiDay, number]> = (() => {
    const arr: Array<[TuiDay, number]> = [];
    let v = 55;
    for (let i = 0; i < this.DAYS; i++) {
      const d = this.start.append({ day: i });
      v += this.rand(-20, 20);
      if (i >= 35 && i <= 38) v -= 25;
      if (i >= 90 && i <= 95) v += 30;
      v = this.clamp(v, 0, 100);
      arr.push([d, Math.round(v)]);
    }
    return arr;
  })();

  protected readonly pressureData: ReadonlyArray<[TuiDay, number]> = (() => {
    const arr: Array<[TuiDay, number]> = [];
    for (let i = 0; i < this.DAYS; i++) {
      const d = this.start.append({ day: i });
      let v = 1015 + 8 * Math.sin(i / 12) + this.rand(-3, 3);
      if (i >= 40 && i <= 45) v -= 35;
      if (i >= 100 && i <= 104) v += 18;
      v = this.clamp(v, 950, 1050);
      arr.push([d, Math.round(v)]);
    }
    return arr;
  })();

  private readonly minDay = this.start;
  private readonly maxDay = this.start.append({ day: this.DAYS - 1 });

  // Dialog
  private readonly dialog$: Observable<TuiDayRange> = this.dialogs.open(
    new PolymorpheusComponent(
      TuiMobileCalendarDropdown,
      Injector.create({
        providers: [{ provide: TUI_CALENDAR_DATE_STREAM, useValue: tuiControlValue(this.control) }],
        parent: this.injector,
      })
    ),
    { size: "fullscreen", closeable: false, data: { min: this.minDay, max: this.maxDay } }
  );

  protected readonly date$ = combineLatest([
    tuiControlValue<TuiDayRange>(this.control),
    this.months$,
  ]).pipe(
    map(([value, months]) => {
      if (!value) return "Choose a date range";
      return value.isSingleDay
        ? `${months[value.from.month]} ${value.from.day}, ${value.from.year}`
        : `${months[value.from.month]} ${value.from.day}, ${value.from.year} - ${months[value.to.month]} ${value.to.day}, ${value.to.year}`;
    })
  );

  protected onClick(): void {
    this.dialog$.subscribe(v => this.control.setValue(v));
  }

  // Utils
  private clamp(n: number, min: number, max: number): number {
    return Math.min(max, Math.max(min, n));
  }
  private rand(min: number, max: number): number {
    return min + Math.random() * (max - min);
  }
}
