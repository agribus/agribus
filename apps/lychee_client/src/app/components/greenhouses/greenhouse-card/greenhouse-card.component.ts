import {
  ChangeDetectionStrategy,
  Component,
  Input,
  Output,
  EventEmitter,
  inject,
} from "@angular/core";
import { TuiSurface, TuiTitle } from "@taiga-ui/core";
import { TUI_CONFIRM, TuiAvatar, type TuiConfirmData } from "@taiga-ui/kit";
import { TuiCardLarge, TuiHeader } from "@taiga-ui/layout";
import { TuiDialogService } from "@taiga-ui/experimental";

@Component({
  selector: "app-greenhouse-card",
  imports: [TuiAvatar, TuiCardLarge, TuiHeader, TuiSurface, TuiTitle],
  templateUrl: "./greenhouse-card.component.html",
  styleUrl: "./greenhouse-card.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class GreenhouseCardComponent {
  @Input()
  public greenhouseTitle: string | undefined;

  @Input()
  public content!: string;

  @Input()
  public yes!: string;

  @Input()
  public no!: string;

  @Input()
  public buttonText!: string;

  @Input()
  public label!: string;

  @Output() public confirmed = new EventEmitter<void>();

  public onConfirmClick() {
    this.confirmed.emit();
  }
  private readonly dialogs = inject(TuiDialogService);

  protected onClick(): void {
    const data: TuiConfirmData = {
      content: this.content,
      yes: this.yes,
      no: this.no,
    };

    this.dialogs
      .open<boolean>(TUI_CONFIRM, { size: "s", label: this.label, data })
      .subscribe(confirmed => {
        if (confirmed) {
          this.onConfirmClick();
        }
      });
  }
}
