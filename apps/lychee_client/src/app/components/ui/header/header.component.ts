import { Component, DestroyRef, inject, OnInit, signal } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { TuiButton, tuiItemsHandlersProvider, TuiTextfield } from "@taiga-ui/core";
import { TuiAppBar } from "@taiga-ui/layout";
import { TuiChevron, TuiDataListWrapper, TuiSelect } from "@taiga-ui/kit";
import { filter, map, switchMap, of } from "rxjs";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { PlatformService } from "@services/platform/platform.service";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { TranslatePipe } from "@ngx-translate/core";
import { DevToolsService } from "@services/dev-tools/dev-tools.service";
import { environment } from "@environment/environment";
import { HeaderType } from "@enums/header-type";
import { HeaderStateService } from "@services/header-state.service";

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
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly platformService = inject(PlatformService);
  private readonly greenhouseService = inject(GreenhouseService);
  private readonly devToolsService = inject(DevToolsService);
  private readonly headerStateService = inject(HeaderStateService);
  private readonly destroyRef = inject(DestroyRef);

  public readonly isMobile = this.platformService.isMobile();
  public readonly greenhouses = this.greenhouseService.getGreenhouses();

  public value: Greenhouse | null = this.greenhouses[0];
  public maxLengthGreenhouse = Math.max(...this.greenhouses.map(g => g.name.length));
  public url: string = "/";
  public showDevTools = environment.devTools;

  public headerType: HeaderType = HeaderType.Default;
  public HeaderType = HeaderType;

  ngOnInit() {
    this.router.events
      .pipe(
        filter(event => event instanceof NavigationEnd),
        map(() => this.getDeepestChild(this.activatedRoute)),
        switchMap(route => of(route.snapshot.data)),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(data => {
        this.headerType = data["headerType"] || HeaderType.Default;
        this.url = this.router.url;

        this.headerStateService.headerType.set(this.headerType);
      });
  }

  private getDeepestChild(route: ActivatedRoute): ActivatedRoute {
    while (route.firstChild) {
      route = route.firstChild;
    }
    return route;
  }

  public gotoPage(pageName: string) {
    this.router.navigate([`/${pageName}`]);
  }

  public back() {
    window.history.back();
  }

  public openDevTools() {
    this.devToolsService.toggle();
  }
}
