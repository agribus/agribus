import { Component, inject } from "@angular/core";
import { TuiButton } from "@taiga-ui/core";
import { Router } from "@angular/router";
import { TranslatePipe } from "@ngx-translate/core";

@Component({
  selector: "app-not-found",
  imports: [TuiButton, TranslatePipe],
  templateUrl: "./not-found.component.html",
  styleUrl: "./not-found.component.scss",
})
export class NotFoundComponent {
  private router = inject(Router);

  goHome() {
    this.router.navigate(["/"]);
  }
}
