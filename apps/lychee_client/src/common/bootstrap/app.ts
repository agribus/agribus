import { TuiRoot } from '@taiga-ui/core';
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HelloWorld } from '../shared/hello-world/hello-world';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, TuiRoot, HelloWorld],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class BootstrapApp {}
