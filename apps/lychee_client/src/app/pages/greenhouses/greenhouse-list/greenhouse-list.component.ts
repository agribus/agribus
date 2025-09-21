import { Component, inject, OnInit } from "@angular/core";
import { GreenhouseCardComponent } from "@components/greenhouses/greenhouse-card/greenhouse-card.component";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { ConfirmComponent } from "@components/settings/confirm/confirm.component";
import { TranslatePipe } from "@ngx-translate/core";
import { Router } from "@angular/router";

@Component({
  selector: "app-greenhouse-list",
  imports: [GreenhouseCardComponent, ConfirmComponent, TranslatePipe],
  templateUrl: "./greenhouse-list.component.html",
  styleUrl: "./greenhouse-list.component.scss",
})
export class GreenhouseListComponent implements OnInit {
  private readonly greenhouseService = inject(GreenhouseService);
  private readonly router = inject(Router);
  protected greenhouses: Greenhouse[] = [];

  ngOnInit() {
    this.loadGreenhouses();
  }

  private loadGreenhouses() {
    this.greenhouseService.loadUserGreenhouses().subscribe(res => {
      this.greenhouses = res.filter(garden => garden !== undefined);
      console.log(this.greenhouses);
    });
  }
  protected handleDeleteGreenhouse(id: string) {
    this.greenhouseService.deleteGreenhouse(id).subscribe();
    this.router.navigate(["/settings/greenhouses"]);
  }
}
