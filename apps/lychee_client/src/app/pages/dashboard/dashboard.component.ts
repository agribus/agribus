import { Component, inject, Injector, INJECTOR, OnInit } from "@angular/core";
import { AsyncPipe, DatePipe, KeyValuePipe } from "@angular/common";
import { FormsModule, FormControl } from "@angular/forms";
import {
  BehaviorSubject,
  combineLatest,
  filter,
  map,
  Observable,
  shareReplay,
  switchMap,
} from "rxjs";

import { TuiCard } from "@taiga-ui/layout";
import {
  TUI_MONTHS,
  TuiAppearance,
  TuiButton,
  TuiDialogService,
  TuiIcon,
  TuiTitle,
} from "@taiga-ui/core";
import { TUI_CALENDAR_DATE_STREAM, TuiBadge, TuiStatus } from "@taiga-ui/kit";
import { TuiMobileCalendarDropdown } from "@taiga-ui/addon-mobile";
import { tuiControlValue, TuiDay, TuiDayRange } from "@taiga-ui/cdk";
import { PolymorpheusComponent } from "@taiga-ui/polymorpheus";
import { ChartDashboard } from "@components/chart-dashboard/chart-dashboard";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { DashboardService } from "@services/dashboard/dashboard.service";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Sensor, SensorData, SummaryAggregates } from "@interfaces/dashboard.interface";
import { ChartTimeseries, MetricPoint } from "@interfaces/chart.interface";

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
    TuiTitle,
    TuiBadge,
    TuiStatus,
    TuiButton,
    FormsModule,
    AsyncPipe,
    ChartDashboard,
    TuiIcon,
    TranslatePipe,
    DatePipe,
    KeyValuePipe,
  ],
  templateUrl: "./dashboard.component.html",
  styleUrls: ["./dashboard.component.scss"],
})
export class DashboardComponent implements OnInit {
  protected readonly temperatureIcon = `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(
    `<svg width="18" height="24" viewBox="0 0 18 24" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M14.875 5.25C13.4253 5.25 12.25 6.42525 12.25 7.875C12.25 9.32475 13.4253 10.5 14.875 10.5C16.3247 10.5 17.5 9.32475 17.5 7.875C17.5 6.42525 16.3247 5.25 14.875 5.25ZM14.875 9C14.2537 9 13.75 8.49632 13.75 7.875C13.75 7.25368 14.2537 6.75 14.875 6.75C15.4963 6.75 16 7.25368 16 7.875C16 8.49632 15.4963 9 14.875 9ZM7 14.3438V8.25C7 7.83579 6.66421 7.5 6.25 7.5C5.83579 7.5 5.5 7.83579 5.5 8.25V14.3438C4.03727 14.7214 3.08356 16.1278 3.27391 17.6265C3.46427 19.1252 4.7393 20.2485 6.25 20.2485C7.7607 20.2485 9.03573 19.1252 9.22609 17.6265C9.41644 16.1278 8.46273 14.7214 7 14.3438ZM6.25 18.75C5.42157 18.75 4.75 18.0784 4.75 17.25C4.75 16.4216 5.42157 15.75 6.25 15.75C7.07843 15.75 7.75 16.4216 7.75 17.25C7.75 18.0784 7.07843 18.75 6.25 18.75ZM10 12.5625V4.5C10 2.42893 8.32107 0.75 6.25 0.75C4.17893 0.75 2.5 2.42893 2.5 4.5V12.5625C0.511202 14.1548 -0.255157 16.8295 0.588618 19.2334C1.43239 21.6373 3.7023 23.2462 6.25 23.2462C8.7977 23.2462 11.0676 21.6373 11.9114 19.2334C12.7552 16.8295 11.9888 14.1548 10 12.5625ZM6.25 21.75C4.28349 21.7502 2.54464 20.4734 1.95598 18.597C1.36731 16.7207 2.0652 14.6795 3.67937 13.5562C3.8814 13.4152 4.00125 13.1839 4 12.9375V4.5C4 3.25736 5.00736 2.25 6.25 2.25C7.49264 2.25 8.5 3.25736 8.5 4.5V12.9375C8.49998 13.1826 8.61969 13.4122 8.82063 13.5525C10.4383 14.6746 11.1387 16.718 10.5496 18.5965C9.96052 20.475 8.21872 21.7525 6.25 21.75Z" fill="#374141"/>
</svg>`
  )}`;

  protected readonly humidityIcon = `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(
    `<svg width="18" height="22" viewBox="0 0 18 22" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M13.3125 4.47656C12.1545 3.13916 10.8511 1.93486 9.42656 0.885938C9.16823 0.704968 8.82427 0.704968 8.56594 0.885938C7.14399 1.9353 5.84317 3.13958 4.6875 4.47656C2.11031 7.43625 0.75 10.5562 0.75 13.5C0.75 18.0563 4.44365 21.75 9 21.75C13.5563 21.75 17.25 18.0563 17.25 13.5C17.25 10.5562 15.8897 7.43625 13.3125 4.47656ZM9 20.25C5.27379 20.2459 2.25413 17.2262 2.25 13.5C2.25 8.13469 7.45031 3.65625 9 2.4375C10.5497 3.65625 15.75 8.13281 15.75 13.5C15.7459 17.2262 12.7262 20.2459 9 20.25ZM14.2397 14.3756C13.8414 16.6005 12.0996 18.3419 9.87469 18.7397C9.83345 18.7463 9.79176 18.7497 9.75 18.75C9.35993 18.7499 9.03506 18.4508 9.00277 18.0621C8.97048 17.6734 9.24155 17.3248 9.62625 17.2603C11.1797 16.9988 12.4978 15.6806 12.7612 14.1244C12.8306 13.7159 13.218 13.4409 13.6266 13.5103C14.0351 13.5797 14.31 13.9671 14.2406 14.3756H14.2397Z" fill="#374141"/>
</svg>`
  )}`;

  protected readonly pressureIcon = `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(
    `<svg width="22" height="15" viewBox="0 0 22 15" fill="none" xmlns="http://www.w3.org/2000/svg">
<path fill-rule="evenodd" clip-rule="evenodd" d="M18.4119 3.56281C16.4483 1.59475 13.7801 0.492189 11 0.5H10.9625C5.19406 0.519688 0.5 5.28125 0.5 11.1059V13.25C0.5 14.0784 1.17157 14.75 2 14.75H20C20.8284 14.75 21.5 14.0784 21.5 13.25V11C21.5078 8.20749 20.3954 5.52849 18.4119 3.56281ZM20 13.25H10.2228L15.3566 6.19063C15.6004 5.85563 15.5266 5.38637 15.1916 5.1425C14.8566 4.89863 14.3873 4.9725 14.1434 5.3075L8.3675 13.25H2V11.1059C2 10.8172 2.01406 10.5322 2.04031 10.25H4.25C4.66421 10.25 5 9.91421 5 9.5C5 9.08579 4.66421 8.75 4.25 8.75H2.30656C3.27406 5.10687 6.43156 2.3525 10.25 2.03188V4.25C10.25 4.66421 10.5858 5 11 5C11.4142 5 11.75 4.66421 11.75 4.25V2.03094C15.5633 2.35185 18.7582 5.04574 19.7188 8.75H17.75C17.3358 8.75 17 9.08579 17 9.5C17 9.91421 17.3358 10.25 17.75 10.25H19.9691C19.9888 10.4984 20 10.7478 20 11V13.25Z" fill="#374141"/>
</svg>`
  )}`;

  // Metrics section
  metrics: SummaryAggregates["metrics"] | undefined;
  protected readonly Math = Math;

  private readonly icons: Record<string, string> = {
    temperature: this.temperatureIcon,
    humidity: this.humidityIcon,
    airPressure: this.pressureIcon,
  };
  getIcon(type: string): string {
    return this.icons[type];
  }

  // Sensor section
  sensors: Sensor[] = [];

  // Alerts section (demo alerts kept)
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

  private readonly dialogs = inject(TuiDialogService);
  private readonly translateService = inject(TranslateService);
  private readonly dashboardService = inject(DashboardService);
  private readonly greenhouseService = inject(GreenhouseService);
  private readonly injector = inject(INJECTOR);
  private readonly months$ = inject(TUI_MONTHS);

  private readonly control = new FormControl<TuiDayRange | null>(null);
  get selectedRange(): TuiDayRange | null {
    return this.control.value;
  }

  private readonly greenhouseId$ = new BehaviorSubject<string | undefined>(undefined);

  protected readonly date$ = combineLatest([
    tuiControlValue<TuiDayRange>(this.control),
    this.months$,
    this.translateService.stream("shared.dashboard.calendar.no-selection"),
  ]).pipe(
    map(([value, months, noSelectionText]) => {
      if (!value) return noSelectionText;
      return value.isSingleDay
        ? `${months[value.from.month]} ${value.from.day}, ${value.from.year}`
        : `${months[value.from.month]} ${value.from.day}, ${value.from.year} - ${months[value.to.month]} ${value.to.day}, ${value.to.year}`;
    })
  );

  // ---- Charts: live data from API ----

  private readonly range$ = tuiControlValue<TuiDayRange>(this.control);

  private readonly chartTimeseries$: Observable<ChartTimeseries> = combineLatest([
    this.greenhouseId$,
    this.range$,
  ]).pipe(
    filter(([id, range]) => !!id && !!range),
    switchMap(([id, range]) =>
      this.dashboardService.getChartsTimeseriesById(
        id as string,
        this.toDateOnly(range!.from),
        this.toDateOnly(range!.to)
      )
    ),
    map(res => res.chartTimeseries),
    shareReplay(1)
  );

  // Per-metric series and meta
  readonly temperatureData$ = this.chartTimeseries$.pipe(
    map(ct => this.toSeries(ct.metrics.temperature.points))
  );
  readonly humidityData$ = this.chartTimeseries$.pipe(
    map(ct => this.toSeries(ct.metrics.humidity.points))
  );
  readonly pressureData$ = this.chartTimeseries$.pipe(
    map(ct => this.toSeries(ct.metrics.airPressure.points))
  );

  readonly tempMeta$ = this.chartTimeseries$.pipe(map(ct => ct.metrics.temperature));
  readonly humMeta$ = this.chartTimeseries$.pipe(map(ct => ct.metrics.humidity));
  readonly presMeta$ = this.chartTimeseries$.pipe(map(ct => ct.metrics.airPressure));

  // Dialog (you can adjust min/max if you want to constrain selection)
  private readonly dialog$: Observable<TuiDayRange> = this.dialogs.open(
    new PolymorpheusComponent(
      TuiMobileCalendarDropdown,
      Injector.create({
        providers: [{ provide: TUI_CALENDAR_DATE_STREAM, useValue: tuiControlValue(this.control) }],
        parent: this.injector,
      })
    ),
    { size: "fullscreen", closeable: false }
  );

  ngOnInit(): void {
    const greenhouseId = this.greenhouseService.selectedSerre()?.id;
    this.greenhouseId$.next(greenhouseId);

    // Measurements (cards)
    this.dashboardService.getGreenhouseMeasurementsById(greenhouseId).subscribe({
      next: (data: SensorData) => {
        this.sensors = data.sensors ?? [];
        this.metrics =
          data.summaryAggregates?.metrics ?? data.summaryAggregates?.metrics ?? undefined;
      },
      error: err => console.error("HTTP error:", err),
    });
  }

  protected onClick(): void {
    this.dialogs
      .open<TuiDayRange>(
        new PolymorpheusComponent(
          TuiMobileCalendarDropdown,
          Injector.create({
            // Provide the current FormControl value/stream to the calendar
            providers: [
              { provide: TUI_CALENDAR_DATE_STREAM, useValue: tuiControlValue(this.control) },
            ],
            parent: this.injector,
          })
        ),
        {
          size: "fullscreen",
          closeable: false,
        }
      )
      .subscribe({
        next: range => {
          if (range) this.control.setValue(range); // <-- APPLY selection
        },
        error: err => console.error("Calendar dialog error", err),
      });
  }

  // ---- Helpers ----

  /** Convert API points into [TuiDay, number] tuples (skips null values) */
  private toSeries(points: MetricPoint[]): ReadonlyArray<[TuiDay, number]> {
    return points
      .filter(p => p.value !== null)
      .map(p => [this.fromDateOnly(p.date), p.value as number] as [TuiDay, number]);
  }

  /** "yyyy-MM-dd" -> TuiDay (UTC-safe) */
  private fromDateOnly(d: string): TuiDay {
    const [y, m, dd] = d.split("-").map(Number);
    return new TuiDay(y, m - 1, dd);
  }

  /** TuiDay -> "yyyy-MM-dd" (UTC-safe) */
  private toDateOnly(day: TuiDay): string {
    const y = day.year;
    const m = String(day.month + 1).padStart(2, "0");
    const d = String(day.day).padStart(2, "0");
    return `${y}-${m}-${d}`;
  }
}
