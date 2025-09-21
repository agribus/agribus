import { inject, Injectable } from "@angular/core";
import { environment } from "@environment/environment";
import { HttpClient } from "@angular/common/http";
import { AuthService } from "@services/auth/auth.service";
import { SensorData } from "@interfaces/dashboard.interface";

@Injectable({
  providedIn: "root",
})
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
}
