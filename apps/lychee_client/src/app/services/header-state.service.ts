import { Injectable, signal } from "@angular/core";
import { HeaderType } from "@enums/header-type";

@Injectable({
  providedIn: "root",
})
export class HeaderStateService {
  public headerType = signal<HeaderType>(HeaderType.Default);
}
