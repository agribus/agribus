import { Component, Input, SimpleChanges, inject, OnChanges } from "@angular/core";
import { AsyncPipe, DatePipe } from "@angular/common";
import { TuiAxes, TuiLineDaysChart } from "@taiga-ui/addon-charts";
import { TUI_MONTHS } from "@taiga-ui/core";
import { TuiDay, TuiDayRange, TuiMonth, tuiPure } from "@taiga-ui/cdk";
import { BehaviorSubject, combineLatest, map } from "rxjs";

@Component({
  selector: "app-chart-dashboard",
  imports: [TuiAxes, TuiLineDaysChart, AsyncPipe, DatePipe],
  templateUrl: "./chart-dashboard.html",
  styleUrl: "./chart-dashboard.scss",
})
export class ChartDashboard implements OnChanges {
  @Input() title = "";
  @Input() unit = "";
  @Input() data: ReadonlyArray<[TuiDay, number]> | null = [];
  @Input() range: TuiDayRange | null = null;
  @Input() yMin = 0;
  @Input() yMax = 100;
  @Input() horizontalLines = 4;

  private readonly months$ = inject(TUI_MONTHS);

  private readonly params$ = new BehaviorSubject<{
    data: ReadonlyArray<[TuiDay, number]>;
    range: TuiDayRange | null;
  }>({
    data: this.data ?? [],
    range: this.range,
  });

  readonly chartData$ = this.params$.pipe(
    map(({ data, range }) => this.filterByRange(data, range))
  );

  readonly labels$ = combineLatest([this.months$, this.chartData$]).pipe(
    map(([months, data]) => {
      if (!data.length) return [] as (string | null)[];
      const from = data[0][0];
      const to = data[data.length - 1][0];
      const monthCount = TuiMonth.lengthBetween(from, to) + 1;
      const labels = Array.from(
        { length: monthCount },
        (_, i) => months[from.append({ month: i }).month] ?? ""
      );
      return [...labels, null] as (string | null)[];
    })
  );

  get y(): number {
    return this.yMin;
  }

  get height(): number {
    return Math.max(this.yMax - this.yMin, 1);
  }

  get labelsY(): ReadonlyArray<string> {
    const steps = Math.max(this.horizontalLines, 0);
    const labels = Array<string>(steps + 1).fill("");
    labels[0] = this.yToLabel(this.yMin);
    labels[steps] = this.yToLabel(this.yMax);
    return labels;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes["data"] || changes["range"]) {
      this.params$.next({ data: this.data ?? [], range: this.range ?? null });
    }
  }

  readonly dayToLabel = (d: TuiDay) => String(d.day);
  readonly yToLabel = (y: number) => `${Math.round(y)} ${this.unit}`;

  toDate(day: TuiDay): Date {
    return day.toLocalNativeDate();
  }

  @tuiPure
  private filterByRange(
    data: ReadonlyArray<[TuiDay, number]>,
    range: TuiDayRange | null
  ): ReadonlyArray<[TuiDay, number]> {
    if (!range) return [];
    const { from, to } = range;
    return data.filter(([day]) => !day.dayBefore(from) && !day.dayAfter(to));
  }
}
