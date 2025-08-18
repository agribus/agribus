import { inject, Injectable, signal } from "@angular/core";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { environment } from "@environment/environment";
import { Crop } from "@interfaces/crop.interface";
import { Sensor } from "@interfaces/sensor.interface";
import { HttpClient } from "@angular/common/http";
import { tap } from "rxjs";

@Injectable({
  providedIn: "root",
})
export class GreenhouseService {
  private readonly http = inject(HttpClient);

  public greenhouses = signal<Greenhouse[]>([]);
  public selectedSerre = signal<Greenhouse | null>(null);

  public loadUserGreenhouses() {
    return this.http
      .get<Greenhouse[]>(`${environment.apiUrl}/greenhouses`, {
        withCredentials: true,
      })
      .pipe(
        tap(greenhouses => {
          this.greenhouses.set(greenhouses);
          if (!this.selectedSerre()) {
            this.selectedSerre.set(greenhouses[0]);
          }
        })
      );
  }

  public loadGreenhouseById(id: string) {
    return this.http
      .get<Greenhouse>(`${environment.apiUrl}/greenhouses/${id}`, {
        withCredentials: true,
      })
      .pipe(tap(serre => this.selectedSerre.set(serre)));
  }

  public createGreenhouse(
    name: string,
    city: string,
    country: string,
    crops: Crop[],
    sensors: Sensor[]
  ) {
    return this.http
      .post<Greenhouse>(
        `${environment.apiUrl}/greenhouses`,
        {
          name: name,
          city: city,
          country: country,
          crops: crops,
          sensors: sensors,
        },
        {
          withCredentials: true,
        }
      )
      .pipe(
        tap(greenhouse => {
          this.greenhouses.update(list => [...list, greenhouse]);
          this.selectedSerre.set(greenhouse);
        })
      );
  }

  public updateGreenhouse(
    id: string,
    name: string,
    city: string,
    country: string,
    crops: Crop[],
    sensors: Sensor[]
  ) {
    return this.http
      .put<Greenhouse>(
        `${environment.apiUrl}/greenhouses/${id}`,
        {
          name: name,
          city: city,
          country: country,
          crops: crops,
          sensors: sensors,
        },
        {
          withCredentials: true,
        }
      )
      .pipe(
        tap(updated => {
          this.greenhouses.update(list => list.map(g => (g.id === id ? updated : g)));
          if (this.selectedSerre()?.id === id) {
            this.selectedSerre.set(updated);
          }
        })
      );
  }

  public deleteGreenhouse(id: string) {
    return this.http
      .delete(`${environment.apiUrl}/greenhouses/${id}`, {
        withCredentials: true,
      })
      .pipe(
        tap(() => {
          this.greenhouses.update(list => list.filter(g => g.id !== id));
          if (this.selectedSerre()?.id === id) {
            this.selectedSerre.set(null);
          }
        })
      );
  }
}
