import { Component, inject, OnInit } from "@angular/core";
import { TuiIcon } from "@taiga-ui/core";
import { TuiSegmented } from "@taiga-ui/kit";
import { NavigationEnd, Router } from "@angular/router";
import { filter } from "rxjs";
import { GreenhouseService } from "@services/greenhouse/greenhouse.service";

@Component({
  selector: "app-navbar",
  imports: [TuiIcon, TuiSegmented],
  templateUrl: "./navbar.component.html",
  styleUrl: "./navbar.component.scss",
})
export class NavbarComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly greenhouseService = inject(GreenhouseService);
  protected activeIndex = 0;

  protected readonly greenhouseSettingsIcon = `data:image/svg+xml;charset=UTF-8,${encodeURIComponent(
    `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
    <path d="M11.3122 21H5C4.46957 21 3.96086 20.7893 3.58579 20.4142C3.21071 20.0391 3 19.5304 3 19V9.99997C2.99993 9.70904 3.06333 9.42159 3.18579 9.15768C3.30824 8.89378 3.4868 8.65976 3.709 8.47197L10.709 2.47297C11.07 2.16788 11.5274 2.00049 12 2.00049C12.4726 2.00049 12.93 2.16788 13.291 2.47297L20.291 8.47197C20.5132 8.65976 20.6918 8.89378 20.8142 9.15768C20.9367 9.42159 21.0001 9.70904 21 9.99997V11.5"/>
    <path d="M14.5987 12.1991C14.4258 12.0699 14.2158 12 14 12H10C9.73478 12 9.48043 12.1054 9.29289 12.2929C9.10536 12.4804 9 12.7348 9 13V21"/>
    <path d="M15 20.225L15.923 19.843"/>
    <path d="M15.923 17.5471L15 17.1641"/>
    <path d="M17.547 15.923L17.164 15"/>
  </svg>`
  )}`;

  ngOnInit() {
    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const url = event.urlAfterRedirects;

        switch (true) {
          case url.includes("dashboard"):
            this.activeIndex = 1;
            break;
          case url.includes("greenhouse"):
            this.activeIndex = 2;
            break;
          case url.includes("settings-account"):
            this.activeIndex = 3;
            break;
          default:
            this.activeIndex = 0;
            break;
        }
      });
  }

  gotoPage(pageName: string) {
    this.router.navigate([`/${pageName}`]);
  }

  goToGreenhouse() {
    if (!this.greenhouseService.selectedSerre()) {
      this.router.navigate(["/greenhouse/create"]);
    } else {
      this.router.navigate(["/greenhouse/settings", this.greenhouseService.selectedSerre()?.id]);
    }
  }
}
