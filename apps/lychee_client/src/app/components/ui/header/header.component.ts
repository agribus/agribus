import { Component, DestroyRef, inject, OnInit, signal } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { TuiButton, TuiDropdown, tuiItemsHandlersProvider, TuiTextfield } from "@taiga-ui/core";
import { TuiAppBar } from "@taiga-ui/layout";
import { TuiChevron, TuiDataListWrapper, TuiSelect } from "@taiga-ui/kit";
import { filter, map, of, switchMap } from "rxjs";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { PlatformService } from "@services/platform/platform.service";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";
import { Greenhouse } from "@interfaces/greenhouse.interface";
import { TranslatePipe, TranslateService } from "@ngx-translate/core";
import { DevToolsService } from "@services/dev-tools/dev-tools.service";
import { environment } from "@environment/environment";
import { HeaderType } from "@enums/header-type";
import { HeaderStateService } from "@services/ui/header-state/header-state.service";
import { AuthService } from "@services/auth/auth.service";

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
    TuiDropdown,
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
  private readonly translateService = inject(TranslateService);
  private readonly authService = inject(AuthService);

  public readonly isMobile = this.platformService.isMobile();
  public greenhouses!: Greenhouse[];

  public value!: Greenhouse | null;
  public maxLengthGreenhouse!: number;
  public url: string = "/";
  public showDevTools = environment.devTools;

  public headerType: HeaderType = HeaderType.Default;
  public HeaderType = HeaderType;

  private lastSelectedGreenhouse: Greenhouse | null = null;

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

        if (this.headerType === HeaderType.Default && this.authService.isLoggedIn()) {
          this.loadGreenhouses();
        }
      });
  }

  private loadGreenhouses() {
    this.greenhouseService
      .getGreenhouses()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(greenhouses => {
        this.greenhouses = greenhouses;
        this.value = this.greenhouses[0] || null;
        this.lastSelectedGreenhouse = this.value;
        this.maxLengthGreenhouse = Math.max(...this.greenhouses.map(g => g.name.length));
      });
  }

  public get greenhouseOptions() {
    return [
      ...this.greenhouses,
      {
        id: "new",
        name: this.translateService.instant("components.ui.header.new-greenhouse"),
        special: true,
      },
    ];
  }

  public onGreenhouseChange(selected: Greenhouse) {
    if ((selected as any).special) {
      this.value = this.lastSelectedGreenhouse;
      this.router.navigate(["/greenhouse/create"]);
    } else {
      this.lastSelectedGreenhouse = selected;
    }
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
