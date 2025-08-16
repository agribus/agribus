import { Component, inject, signal } from "@angular/core";
import { GreenhouseFormComponent } from "@components/greenhouses/greenhouse-form/greenhouse-form.component";
import { ActivatedRoute } from "@angular/router";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { filter, map, switchMap } from "rxjs";

@Component({
  selector: "app-greenhouse-settings",
  imports: [GreenhouseFormComponent],
  templateUrl: "./greenhouse-settings.component.html",
  styleUrl: "./greenhouse-settings.component.scss",
})
export class GreenhouseSettingsComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly greenhouseService = inject(GreenhouseService);

  public greenhouse = signal<Greenhouse | null>(null);

  constructor() {
    this.route.paramMap
      .pipe(
        takeUntilDestroyed(),
        map(params => params.get("id")),
        filter(id => !!id),
        switchMap(id => {
          this.greenhouse.set(null);
          return this.greenhouseService.loadGreenhouseById(id!);
        })
      )
      .subscribe(g => this.greenhouse.set(g));
  }
}
