import { Component, Input } from "@angular/core";
import { TuiButton } from "@taiga-ui/core";

@Component({
  selector: "app-go-to-card",
  imports: [TuiButton],
  templateUrl: "./go-to-card.component.html",
  styleUrl: "./go-to-card.component.scss",
})
export class GoToCardComponent {
  @Input()
  public text!: string;
}
