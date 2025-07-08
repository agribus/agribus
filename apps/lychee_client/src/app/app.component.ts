import { TuiRoot } from '@taiga-ui/core';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HelloWorldComponent } from '@components/hello-world/hello-world.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot, HelloWorldComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss',
})
export class BootstrapApp {}
