import { TuiRoot } from '@taiga-ui/core';
import { Component, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HelloWorldComponent } from '@components/hello-world/hello-world.component';
import {TranslateModule} from "@ngx-translate/core";
import {TranslateService} from "@ngx-translate/core";


@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot, HelloWorldComponent, TranslateModule,],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class BootstrapApp {

  private readonly translateService = inject(TranslateService);
  constructor() {
    this.translateService.addLangs(['fr', 'en']);
    this.translateService.setDefaultLang('fr');
    this.translateService.use('fr');
  }
}
