import { Component, signal } from "@angular/core";
import { TuiBadge, TuiStatus } from "@taiga-ui/kit";
import { Platform } from "../../core/services/platform/platform";

@Component({
  selector: "shared-hello-world",
  imports: [TuiBadge, TuiStatus],
  templateUrl: "./hello-world.html",
  styleUrl: "./hello-world.scss",
})
export class HelloWorld {
  public message = signal<string>("");

  constructor(private readonly platform: Platform) {
    this.message.set(
      this.platform.isBrowser() ? "Hello from the browser!" : "Hello from a mobile device!"
    );
  }
}
