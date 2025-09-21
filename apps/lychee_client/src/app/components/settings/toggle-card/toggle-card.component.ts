import { Component, Input, Output, EventEmitter, inject } from "@angular/core";
import { FormsModule } from "@angular/forms";
import { TuiSwitch } from "@taiga-ui/kit";

@Component({
  selector: "app-toggle-card",
  imports: [FormsModule, TuiSwitch],
  templateUrl: "./toggle-card.component.html",
  styleUrl: "./toggle-card.component.scss",
})
export class ToggleCardComponent {
  @Input()
  public subTitle: string = "";

  @Input()
  public text: string = "";

  @Input()
  public isActive: boolean = false;

  @Output() public toggled = new EventEmitter<boolean>();
}
