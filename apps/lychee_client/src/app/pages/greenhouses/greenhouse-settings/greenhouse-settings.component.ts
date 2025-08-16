import { Component, inject, signal } from "@angular/core";
import { GreenhouseFormComponent } from "@components/greenhouses/greenhouse-form/greenhouse-form.component";
import { ActivatedRoute } from "@angular/router";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Component({
  selector: "app-greenhouse-settings",
  imports: [GreenhouseFormComponent],
  templateUrl: "./greenhouse-settings.component.html",
  styleUrl: "./greenhouse-settings.component.scss",
})
export class GreenhouseSettingsComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly greenhouseService = inject(GreenhouseService);

  public isEditMode = signal(false);
  public greenhouse = signal<Greenhouse | null>(null);

  constructor() {
    const id = this.route.snapshot.paramMap.get("id");
    console.log(id);
    if (id) {
      this.isEditMode.set(true);
      this.greenhouseService
        .loadGreenhouseById(id)
        .pipe(takeUntilDestroyed())
        .subscribe(g => this.greenhouse.set(g));
    }
  }
}
