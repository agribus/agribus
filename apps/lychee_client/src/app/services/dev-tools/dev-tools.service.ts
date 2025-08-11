import { Injectable, signal } from "@angular/core";

@Injectable({
  providedIn: "root",
})
export class DevToolsService {
  public open = signal(false);

  toggle() {
    this.open.set(!this.open());
  }
}
