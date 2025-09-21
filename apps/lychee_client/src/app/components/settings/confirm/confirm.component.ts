import {
  ChangeDetectionStrategy,
  Component,
  inject,
  Input,
  Output,
  EventEmitter,
} from "@angular/core";
import { TuiButton } from "@taiga-ui/core";
import { TuiDialogService } from "@taiga-ui/experimental";
import { TUI_CONFIRM, type TuiConfirmData } from "@taiga-ui/kit";

@Component({
  selector: "app-confirm",
  imports: [TuiButton],
  templateUrl: "./confirm.component.html",
  styleUrl: "./confirm.component.scss",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmComponent {
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
