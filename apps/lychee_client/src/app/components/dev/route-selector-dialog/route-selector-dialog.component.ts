import { Component, inject } from "@angular/core";
import { TuiButton, TuiDialogContext, TuiTextfield } from "@taiga-ui/core";
import { Router } from "@angular/router";
import { injectContext } from "@taiga-ui/polymorpheus";
import { TuiChevron, TuiDataListWrapper, TuiSelect } from "@taiga-ui/kit";
import { FormsModule } from "@angular/forms";

@Component({
  selector: "app-route-selector-dialog",
  imports: [TuiSelect, FormsModule, TuiDataListWrapper, TuiButton, TuiTextfield, TuiChevron],
  templateUrl: "./route-selector-dialog.component.html",
  styleUrl: "./route-selector-dialog.component.scss",
})
export class RouteSelectorDialogComponent {
  private readonly router = inject(Router);
  public readonly context = injectContext<TuiDialogContext<void, void>>();

  public routes: string[] = [];
  public selectedRoute: string = "";

  constructor() {
    const config = this.router.config;
    this.routes = config.map(r => "/" + r.path).filter(path => path !== "/");
    console.log(this.routes);
  }

  public navigate(): void {
    this.router.navigateByUrl(this.selectedRoute);
    this.context.completeWith();
  }
}
