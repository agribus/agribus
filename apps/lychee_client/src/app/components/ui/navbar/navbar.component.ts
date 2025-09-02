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
    `<svg width="25" height="24" viewBox="0 0 25 24" fill="none" xmlns="http://www.w3.org/2000/svg">
<path d="M11.8122 21H5.5C4.96957 21 4.46086 20.7893 4.08579 20.4142C3.71071 20.0392 3.5 19.5305 3.5 19V10C3.49993 9.7091 3.56333 9.42165 3.68579 9.15775C3.80824 8.89384 3.9868 8.65983 4.209 8.47203L11.209 2.47303C11.57 2.16794 12.0274 2.00055 12.5 2.00055C12.9726 2.00055 13.43 2.16794 13.791 2.47303L20.791 8.47203C21.0132 8.65983 21.1918 8.89384 21.3142 9.15775C21.4367 9.42165 21.5001 9.7091 21.5 10V11.5001" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M15.0987 12.1991C14.9258 12.0699 14.7158 12 14.5 12H10.5C10.2348 12 9.98043 12.1054 9.79289 12.2929C9.60536 12.4804 9.5 12.7348 9.5 13V21" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M15.5 20.225L16.423 19.843" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M16.423 17.5471L15.5 17.1641" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M18.047 15.923L17.664 15" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M18.047 21.467L17.664 22.391" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M20.343 15.923L20.726 15" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M20.725 22.391L20.343 21.467" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M21.967 17.5471L22.891 17.1641" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M21.967 19.843L22.891 20.226" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
<path d="M19.195 21.6951C20.8519 21.6951 22.195 20.3519 22.195 18.6951C22.195 17.0382 20.8519 15.6951 19.195 15.6951C17.5382 15.6951 16.195 17.0382 16.195 18.6951C16.195 20.3519 17.5382 21.6951 19.195 21.6951Z" stroke="#374141" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/>
</svg>
`
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
