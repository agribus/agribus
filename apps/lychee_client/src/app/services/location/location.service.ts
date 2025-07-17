import { inject, Injectable } from "@angular/core";
import { catchError, map, Observable, of } from "rxjs";
import { HttpClient } from "@angular/common/http";
import { UserLocation } from "@interfaces/user-location.interface";

interface NominatimResponse {
  lat: string;
  lon: string;
  address: {
    country: string;
    country_code: string;
  };
}

@Injectable({
  providedIn: "root",
})
export class LocationService {
  private http = inject(HttpClient);

  constructor() {}

  public getLocationFromCoords(lat: number, lon: number): Observable<UserLocation | null> {
    const url = `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}&zoom=3&addressdetails=1`;

    return this.http.get<NominatimResponse>(url).pipe(
      map(data => {
        if (!data?.address) return null;

        return {
          country: data.address.country,
          countryCode: (data.address.country_code || "").toUpperCase(),
          latitude: lat,
          longitude: lon,
        } as UserLocation;
      }),
      catchError(() => of(null))
    );
  }
}
