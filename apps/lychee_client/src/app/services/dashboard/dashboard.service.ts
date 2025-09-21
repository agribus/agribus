import { inject, Injectable } from "@angular/core";
import { environment } from "@environment/environment";
import { HttpClient, HttpParams } from "@angular/common/http";
import { AuthService } from "@services/auth/auth.service";
import { SensorData } from "@interfaces/dashboard.interface";
import { ChartTimeseriesResponse } from "@interfaces/chart.interface";

@Injectable({ providedIn: "root" })
export class DashboardService {
  private readonly http = inject(HttpClient);
  private readonly authService = inject(AuthService);

  public getGreenhouseMeasurementsById(id: string | undefined) {
    return this.http.get<SensorData>(`${environment.apiUrl}/greenhouses/${id}/measurements`, {
      headers: {
        Authorization: `Bearer ${this.authService.token()}`,
      },
    });
  }

  public getChartsTimeseriesById(id: string | undefined, from: string | Date, to: string | Date) {
    const params = new HttpParams()
      .set("from", this.toDateOnly(from))
      .set("to", this.toDateOnly(to));

    return this.http.get<ChartTimeseriesResponse>(
      `${environment.apiUrl}/greenhouses/${id}/charts/timeseries`,
      {
        headers: { Authorization: `Bearer ${this.authService.token()}` },
        params,
      }
    );
  }

  private toDateOnly(input: string | Date): string {
    if (typeof input === "string") return input;
    const y = input.getUTCFullYear();
    const m = String(input.getUTCMonth() + 1).padStart(2, "0");
    const d = String(input.getUTCDate()).padStart(2, "0");
    return `${y}-${m}-${d}`;
  }
}
