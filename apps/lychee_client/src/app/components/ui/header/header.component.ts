import { Component, DestroyRef, inject, OnInit, signal } from "@angular/core";
import { ActivatedRoute, NavigationEnd, Router } from "@angular/router";
import { FormsModule } from "@angular/forms";
import {
  TUI_DARK_MODE,
  TuiButton,
  TuiDropdown,
  tuiItemsHandlersProvider,
  TuiTextfield,
} from "@taiga-ui/core";
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
import { NgOptimizedImage } from "@angular/common";
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
    NgOptimizedImage,
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
  protected readonly darkMode = inject(TUI_DARK_MODE);

  public readonly isMobile = this.platformService.isMobile();
  public greenhouses!: Greenhouse[];

  public value!: Greenhouse | null;
  public maxLengthGreenhouse!: number;
  public url: string = "/";
  public showDevTools = environment.devTools;

  public headerType: HeaderType = HeaderType.None;
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

        if (this.headerType === HeaderType.Default && this.authService.isLoggedIn()) {
          this.loadGreenhouses();
        }
      });
  }

  private loadGreenhouses() {
    this.greenhouseService.loadUserGreenhouses().subscribe({
      next: () => {
        this.greenhouses = this.greenhouseService.greenhouses();
        this.value = this.greenhouseService.selectedSerre();
        this.maxLengthGreenhouse = Math.max(
          ...this.greenhouseService.greenhouses().map(g => g.name.length)
        );
      },
      error: err => {
        console.error("Erreur lors du chargement des serres", err);
      },
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

  public onGreenhouseChange(greenhouse: Greenhouse) {
    this.greenhouseService.selectedSerre.set(greenhouse);
  }
}
