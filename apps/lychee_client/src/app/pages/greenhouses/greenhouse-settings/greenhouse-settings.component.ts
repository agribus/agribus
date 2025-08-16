import { Component, inject, signal } from "@angular/core";
import { GreenhouseFormComponent } from "@components/greenhouses/greenhouse-form/greenhouse-form.component";
import { ActivatedRoute } from "@angular/router";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";

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
  public greenhouse = signal<Greenhouse | undefined>(undefined);

  constructor() {
    const id = this.route.snapshot.paramMap.get("id");
    console.log(id);
    if (id) {
      this.isEditMode.set(true);
      this.loadGreenhouse(id);
    }
  }

  private loadGreenhouse(id: string): void {
    // this.greenhouseService.getById(id).subscribe(greenhouse => {
    //   this.greenhouse.set(greenhouse);
    // });
  }
}
