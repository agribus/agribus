import { Component, inject, Input, OnInit, signal } from "@angular/core";
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
import { DevToolsService } from "@services/dev-tools/dev-tools.service";
import { environment } from "@environment/environment";

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
  private readonly devToolsService = inject(DevToolsService);

  public readonly isMobile = this.platformService.isBrowser();
  public readonly greenhouses = this.greenhouseService.getGreenhouses();

  public value: Greenhouse | null = this.greenhouses[0];
  public maxLengthGreenhouse = Math.max(...this.greenhouses.map(g => g.name.length));
  public url: string = "/";
  public showDevTools = environment.devTools;

  @Input() isSettingsHeader!: boolean;

  ngOnInit() {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        this.url = event.urlAfterRedirects;
      });
  }

  gotoPage(pageName: string) {
    this.router.navigate([`/${pageName}`]);
  }

  back() {
    window.history.back();
  }

  openDevTools() {
    this.devToolsService.toggle();
  }
}
