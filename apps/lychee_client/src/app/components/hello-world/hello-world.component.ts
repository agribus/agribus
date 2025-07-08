import { Component, inject, signal } from '@angular/core';
import { TuiBadge, TuiStatus } from '@taiga-ui/kit';
import { PlatformService } from '@services/platform/platform.service';

@Component({
  selector: 'app-hello-world',
  imports: [TuiBadge, TuiStatus],
  templateUrl: './hello-world.component.html',
  styleUrl: './hello-world.component.scss',
})
export class HelloWorldComponent {
  public message = signal<string>('');

  private readonly platformService = inject(PlatformService);

  constructor() {
    this.message.set(this.platformService.isBrowser() ? 'Hello from the browser!' : 'Hello from a mobile device!');
  }
}
