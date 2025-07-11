import { Component, inject, OnInit, signal } from "@angular/core";
import { NavigationEnd, Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { TuiButton, tuiItemsHandlersProvider, TuiTextfield } from "@taiga-ui/core";
import { TuiAppBar } from "@taiga-ui/layout";
import { TuiChevron, TuiDataListWrapper, TuiSelect } from "@taiga-ui/kit";
import { filter } from "rxjs";
import { PlatformService } from "@services/platform/platform.service";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { TranslatePipe } from "@ngx-translate/core";

@Component({
  selector: "app-header",
  imports: [
    TuiAppBar,
    TuiButton,
    FormsModule,
    TuiChevron,
    TuiDataListWrapper,
    TuiSelect,
    TuiTextfield,
    TranslatePipe,
  ],
  templateUrl: "./header.component.html",
  styleUrl: "./header.component.scss",
  providers: [
    tuiItemsHandlersProvider({
      stringify: signal((x: Greenhouse) => x.name),
      identityMatcher: signal((a: Greenhouse, b: Greenhouse) => a.id === b.id),
    }),
  ],
})
export class HeaderComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly platformService = inject(PlatformService);
  private readonly greenhouseService = inject(GreenhouseService);
  public readonly isMobile = this.platformService.isBrowser();
  protected isSettingsHeader = false;
  public readonly greenhouses = this.greenhouseService.getGreenhouses();
  public value: Greenhouse | null = this.greenhouses[0];
  public maxLengthGreenhouse = Math.max(...this.greenhouses.map(g => g.name.length));
  public url: string = "/";

  ngOnInit() {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const url = event.urlAfterRedirects;
        this.isSettingsHeader = url.includes("settings") || url.includes("form");
        this.url = url;
      });
  }

  gotoPage(pageName: string) {
    this.router.navigate([`/${pageName}`]);
  }
  back() {
    window.history.back();
  }
}
